using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;

namespace ModdersToolkit
{
	internal static class Utilities
	{
		internal static void LoadProjectile(int type) {
			// Use this instead of Main.instance.LoadProjectile because we don't need ImmediateLoad
			if (TextureAssets.Projectile[type].State == AssetState.NotLoaded)
				Main.Assets.Request<Texture2D>(TextureAssets.Projectile[type].Name, AssetRequestMode.AsyncLoad);
		}

		internal static void LoadItem(int type) {
			// Use this instead of Main.instance.LoadItem because we don't need ImmediateLoad
			if (TextureAssets.Item[type].State == AssetState.NotLoaded)
				Main.Assets.Request<Texture2D>(TextureAssets.Item[type].Name, AssetRequestMode.AsyncLoad);
		}

		internal static void LoadNPC(int type) {
			// Use this instead of Main.instance.LoadNPC because we don't need ImmediateLoad
			if (TextureAssets.Npc[type].State == AssetState.NotLoaded)
				Main.Assets.Request<Texture2D>(TextureAssets.Npc[type].Name, AssetRequestMode.AsyncLoad);
		}
	}
}
