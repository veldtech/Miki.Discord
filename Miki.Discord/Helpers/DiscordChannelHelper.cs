namespace Miki.Discord.Helpers
{
    using System.Linq;
    using Miki.Discord.Common;
    using Miki.Discord.Internal;
    using System.Threading.Tasks;

    public static class DiscordChannelHelper
    {
        public static async Task<DiscordMessage> CreateMessageAsync(
            IDiscordClient client,
            DiscordChannelPacket channel,
            MessageArgs args)
        {
            var message = await client.ApiClient.SendMessageAsync(channel.Id, args);
            if(channel.Type == ChannelType.GuildText
                || channel.Type == ChannelType.GuildVoice
                || channel.Type == ChannelType.GuildCategory
                || channel.Type == ChannelType.GuildNews
                || channel.Type == ChannelType.GuildStore)
            {
                message.GuildId = channel.GuildId;
            }
            return new DiscordMessage(message, client);
        }

        public static GuildPermission GetOverwritePermissions(
            IDiscordGuildUser user,
            IDiscordGuildChannel channel,
            GuildPermission basePermissions)
        {
            var permissions = basePermissions;
            if(permissions.HasFlag(GuildPermission.Administrator))
            {
                return GuildPermission.All;
            }

            if(channel.PermissionOverwrites != null)
            {
                PermissionOverwrite overwriteEveryone = channel.PermissionOverwrites
                    .FirstOrDefault(x => x.Id == channel.GuildId);

                if(overwriteEveryone != null)
                {
                    permissions &= ~overwriteEveryone.DeniedPermissions;
                    permissions |= overwriteEveryone.AllowedPermissions;
                }

                PermissionOverwrite overwrites = new PermissionOverwrite();

                if(user.RoleIds != null)
                {
                    foreach(ulong roleId in user.RoleIds)
                    {
                        PermissionOverwrite roleOverwrites = channel.PermissionOverwrites
                            .FirstOrDefault(x => x.Id == roleId);

                        if(roleOverwrites != null)
                        {
                            overwrites.AllowedPermissions |= roleOverwrites.AllowedPermissions;
                            overwrites.DeniedPermissions &= roleOverwrites.DeniedPermissions;
                        }
                    }
                }

                permissions &= ~overwrites.DeniedPermissions;
                permissions |= overwrites.AllowedPermissions;

                PermissionOverwrite userOverwrite = channel.PermissionOverwrites
                    .FirstOrDefault(x => x.Id == user.Id);

                if(userOverwrite != null)
                {
                    permissions &= ~userOverwrite.DeniedPermissions;
                    permissions |= userOverwrite.AllowedPermissions;
                }
            }

            return permissions;
        }
    }
}
