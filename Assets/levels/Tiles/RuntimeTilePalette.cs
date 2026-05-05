using MarioGame.Utils.Runtime;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MarioGame.Levels.Tiles
{
    public sealed class RuntimeTilePalette
    {
        public Tile Solid { get; }
        public Tile Breakable { get; }
        public Tile Spike { get; }
        public Tile Fire { get; }

        public RuntimeTilePalette(WorldThemeId theme)
        {
            Solid = CreateTile(RuntimeSprites.Square, WorldTheme.GroundColor(theme));
            Breakable = CreateTile(RuntimeSprites.Square, new Color(0.65f, 0.4f, 0.2f));
            Spike = CreateTile(RuntimeSprites.Square, theme == WorldThemeId.Lava ? new Color(1f, 0.35f, 0.15f) : new Color(0.85f, 0.85f, 0.9f));
            Fire = CreateTile(RuntimeSprites.Square, new Color(1f, 0.45f, 0.15f));
        }

        public Tile GetTile(MarioGame.Levels.Data.TileId tileId)
        {
            return tileId switch
            {
                MarioGame.Levels.Data.TileId.Solid => Solid,
                MarioGame.Levels.Data.TileId.Breakable => Breakable,
                MarioGame.Levels.Data.TileId.Spike => Spike,
                MarioGame.Levels.Data.TileId.Fire => Fire,
                _ => null
            };
        }

        private static Tile CreateTile(Sprite sprite, Color color)
        {
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            tile.color = color;
            return tile;
        }
    }
}

