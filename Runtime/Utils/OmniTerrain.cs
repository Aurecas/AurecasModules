using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AurecasLib.Utils {
    [System.Serializable]
    [CreateAssetMenu(fileName = "AutoTile", menuName = "Tiles/AutoTile Omni")]
    public class OmniTerrain : TileBase {
        // Types
        public enum TileDistortion { None, FlipHorizontal, FlipVertical, RotateCW, RotateCCW, Rotate180 }
        public enum TileID {
            Standard, TopLeft, TopCenter, TopRight, MiddleLeft, MiddleCenter, MiddleRight, BottomLeft, BottomCenter,
            BottomRight, InnerTopLeft, InnerTopRight, InnerBottomLeft, InnerBottomRight, DiagonalLeft, DiagonalRight, SingleLeft,
            SingleCenter, SingleRight, SingleTop, SingleMiddle, SingleBottom, Single, SHTopLeft, SHTopRight, SHMiddleLeft, SHMiddleRight,
            SHBottomLeft, SHBottomRight, SVTopLeft, SVTopCenter, SVTopRight, SVBottomLeft, SVBottomCenter, SVBottomRight, SSTopLeft,
            SSTopCenter, SSTopRight, SSMiddleLeft, SSMiddleCenter, SSMiddleRight, SSBottomLeft, SSBottomCenter, SSBottomRight,
            SITopLeft, SITopRight, SIBottomLeft, SIBottomRight
        }
        public enum FallbackPriority { Horizontal, Vertical, Custom }
        public enum FallbackRespect { None, Top, Bottom, Both, Best }
        public enum FallbackForce { None, Horizontal, Vertical, Both }
        public enum ColliderType { Standard, None, Sprite, Grid }
        enum RenderStage { Standard, Force, Exceptions }
        [System.Serializable]
        public class TileVariation {
            public Sprite sprite;
            public float chance = 30;
        }
        [System.Serializable]
        public class TileProperties {
            public Sprite sprite;
            public TileDistortion distortion;
            public Matrix4x4 transform = Matrix4x4.identity;
            public ColliderType collider;
            public List<TileVariation> variations;
            public int fallback;
            public bool variationsExpanded;
        }
        struct TileInstance {
            public int id;
        }

        // Constant
        readonly int[] tileTable = new int[] {
            22,
            22, 21, 21, 22, 22, 21, 21, 18,
            18, 43, 09, 18, 18, 43, 09, 16,
            16, 41, 41, 16, 16, 07, 07, 17,
            17, 42, 27, 17, 17, 28, 08, 22,

            22, 21, 21, 22, 22, 21, 21, 18,
            18, 43, 09, 18, 18, 43, 09, 16,
            16, 41, 41, 16, 16, 07, 07, 17,
            17, 42, 27, 17, 17, 28, 08, 19,

            19, 20, 20, 19, 19, 20, 20, 37,
            37, 40, 31, 37, 37, 40, 31, 35,
            35, 38, 38, 35, 35, 29, 29, 36,
            36, 39, 47, 36, 36, 46, 30, 19,

            19, 20, 20, 19, 19, 20, 20, 03,
            03, 34, 06, 03, 03, 34, 06, 35,
            35, 38, 38, 35, 35, 29, 29, 23,
            23, 45, 25, 23, 23, 15, 10, 22,

            22, 21, 21, 22, 22, 21, 21, 18,
            18, 43, 09, 18, 18, 43, 09, 16,
            16, 41, 41, 16, 16, 07, 07, 17,
            17, 42, 27, 17, 17, 28, 08, 22,

            22, 21, 21, 22, 22, 21, 21, 18,
            18, 43, 09, 18, 18, 43, 09, 16,
            16, 41, 41, 16, 16, 07, 07, 17,
            17, 42, 27, 17, 17, 28, 08, 19,

            19, 20, 20, 19, 19, 20, 20, 37,
            37, 40, 31, 37, 37, 40, 31, 01,
            01, 32, 32, 01, 01, 04, 04, 24,
            24, 44, 14, 24, 24, 26, 11, 19,

            19, 20, 20, 19, 19, 20, 20, 03,
            03, 34, 06, 03, 03, 34, 06, 01,
            01, 32, 32, 01, 01, 04, 04, 02,
            02, 33, 12, 02, 02, 13, 05
        };
        readonly int[][][] fallbackTable = new int[][][] {
            new int[][] { // prioritize: horizontal
                new int[] { // respect: none
                    /*00*/ 00, 02, 05, 02, 05,   /*05*/ 00, 05, 04, 05, 06,
                    /*10*/ 05, 05, 05, 05, 12,   /*15*/ 13, 01, 02, 03, 02,
                    /*20*/ 05, 08, 19, 02, 02,   /*25*/ 12, 13, 08, 08, 04,
                    /*30*/ 05, 06, 04, 05, 06,   /*35*/ 01, 02, 03, 32, 33,
                    /*40*/ 34, 07, 08, 09, 33,   /*45*/ 33, 30, 30
                },
                new int[] { // respect: top
                    /*00*/ 00, 02, 05, 02, 05,   /*05*/ 00, 05, 04, 05, 06,
                    /*10*/ 05, 05, 05, 05, 12,   /*15*/ 13, 01, 02, 03, 02,
                    /*20*/ 05, 08, 19, 02, 02,   /*25*/ 12, 13, 09, 07, 04,
                    /*30*/ 05, 06, 01, 02, 03,   /*35*/ 01, 02, 03, 32, 33,
                    /*40*/ 34, 16, 17, 18, 33,   /*45*/ 33, 13, 12
                },
                new int[] { // respect: bottom
                    /*00*/ 00, 02, 05, 02, 05,   /*05*/ 00, 05, 04, 05, 06,
                    /*10*/ 05, 05, 05, 05, 11,   /*15*/ 10, 07, 08, 09, 02,
                    /*20*/ 05, 08, 21, 03, 01,   /*25*/ 10, 11, 08, 08, 07,
                    /*30*/ 08, 09, 04, 05, 06,   /*35*/ 16, 17, 18, 29, 30,
                    /*40*/ 31, 07, 08, 09, 11,   /*45*/ 10, 30, 30
                },
                new int[] { // respect: both
                    /*00*/ 00, 02, 05, 02, 05,   /*05*/ 00, 05, 04, 05, 06,
                    /*10*/ 05, 05, 05, 05, 01,   /*15*/ 03, 01, 02, 03, 02,
                    /*20*/ 05, 08, 19, 03, 01,   /*25*/ 06, 04, 09, 07, 07,
                    /*30*/ 08, 09, 01, 02, 03,   /*35*/ 16, 17, 18, 20, 36,
                    /*40*/ 20, 16, 17, 18, 24,   /*45*/ 23, 28, 27
                },
                new int[] { // respect: best
                    /*00*/ 00, 02, 05, 02, 05,   /*05*/ 00, 05, 04, 05, 06,
                    /*10*/ 05, 05, 05, 05, 12,   /*15*/ 13, 01, 02, 03, 02,
                    /*20*/ 05, 08, 19, 03, 01,   /*25*/ 06, 04, 09, 07, 07,
                    /*30*/ 08, 09, 01, 02, 03,   /*35*/ 16, 17, 18, 20, 36,
                    /*40*/ 20, 16, 17, 18, 24,   /*45*/ 23, 28, 27
                }
            },
            new int[][] { // prioritize: vertical
                new int[] { // respect: none
                    /*00*/ 00, 02, 05, 02, 05,   /*05*/ 00, 05, 04, 05, 06,
                    /*10*/ 05, 05, 05, 05, 12,   /*15*/ 13, 01, 02, 03, 02,
                    /*20*/ 05, 08, 19, 02, 02,   /*25*/ 12, 13, 08, 08, 04,
                    /*30*/ 05, 06, 04, 05, 06,   /*35*/ 01, 02, 03, 32, 33,
                    /*40*/ 34, 07, 08, 09, 33,   /*45*/ 33, 30, 30
                },
                new int[] { // respect: top
                    /*00*/ 00, 02, 05, 02, 05,   /*05*/ 00, 05, 04, 05, 06,
                    /*10*/ 05, 05, 05, 05, 12,   /*15*/ 13, 01, 02, 03, 02,
                    /*20*/ 05, 08, 19, 02, 02,   /*25*/ 12, 13, 09, 07, 04,
                    /*30*/ 05, 06, 01, 02, 03,   /*35*/ 01, 02, 03, 32, 20,
                    /*40*/ 34, 21, 17, 21, 33,   /*45*/ 33, 13, 12
                },
                new int[] { // respect: bottom
                    /*00*/ 00, 02, 05, 02, 05,   /*05*/ 00, 05, 04, 05, 06,
                    /*10*/ 05, 05, 05, 05, 11,   /*15*/ 10, 07, 08, 09, 02,
                    /*20*/ 05, 08, 21, 03, 01,   /*25*/ 10, 11, 08, 08, 07,
                    /*30*/ 08, 09, 04, 05, 06,   /*35*/ 19, 17, 19, 29, 20,
                    /*40*/ 31, 07, 08, 09, 11,   /*45*/ 10, 30, 30
                },
                new int[] { // respect: both
                    /*00*/ 00, 02, 05, 02, 05,   /*05*/ 00, 05, 04, 05, 06,
                    /*10*/ 05, 05, 05, 05, 01,   /*15*/ 03, 01, 02, 03, 02,
                    /*20*/ 05, 08, 19, 03, 01,   /*25*/ 06, 04, 09, 07, 07,
                    /*30*/ 08, 09, 01, 02, 03,   /*35*/ 19, 17, 19, 20, 20,
                    /*40*/ 20, 21, 17, 21, 32,   /*45*/ 34, 29, 31
                },
                new int[] { // respect: best
                    /*00*/ 00, 02, 05, 02, 05,   /*05*/ 00, 05, 04, 05, 06,
                    /*10*/ 05, 05, 05, 05, 12,   /*15*/ 13, 01, 02, 03, 02,
                    /*20*/ 05, 08, 19, 03, 01,   /*25*/ 06, 04, 09, 07, 07,
                    /*30*/ 08, 09, 01, 02, 03,   /*35*/ 19, 17, 19, 20, 20,
                    /*40*/ 20, 21, 17, 21, 32,   /*45*/ 34, 29, 31
                }
            }
        };
        readonly int[] forceTable = new int[] {
            /*00*/ 00, 11, 01, 21, 10,   /*05*/ 00, 20, 12, 02, 22,
            /*10*/ 00, 00, 00, 00, 00,   /*15*/ 00, 13, 03, 23, 31,
            /*20*/ 30, 32, 33, 01, 01,   /*25*/ 00, 00, 02, 02, 10,
            /*30*/ 00, 20, 10, 00, 20,   /*35*/ 11, 01, 21, 10, 00,
            /*40*/ 20, 12, 02, 22, 00,   /*45*/ 00, 00, 00
        };

        // Properties
        public TileProperties[] tiles;
        public Color baseColor;
        public List<TileBase> allowedTiles;
        public Tile.ColliderType colliderType = Tile.ColliderType.Grid;
        public FallbackRespect fallbackRespect = FallbackRespect.Best;
        public FallbackForce fallbackForce;
        public FallbackPriority fallbackPriority;
        public bool forceHorizontal, forceVertical;

        // Internal
        RenderStage renderStage;

        public override void RefreshTile(Vector3Int location, ITilemap tileMap) {
            // Force: needs to refresh further away
            int stages = fallbackForce == FallbackForce.None ? 1 : 3;

            // Refresh all tiles around it
            for (int s = 0; s < stages; s++) {
                switch (s) {
                    case 0:
                        renderStage = RenderStage.Standard;
                        break;
                    case 1:
                    case 2:
                        renderStage = RenderStage.Force;
                        break;
                }
                int size = 1;

                for (int yd = -size; yd <= size; yd++) {
                    for (int xd = -size; xd <= size; xd++) {
                        Vector3Int position = new Vector3Int(location.x + xd, location.y + yd, location.z);
                        if (HasTileAt(tileMap, position)) {
                            tileMap.RefreshTile(position);
                        }
                    }
                }
            }
        }

        public int GetFallback(int id) {
            if (fallbackPriority == FallbackPriority.Custom) {
                return tiles[id].fallback;
            }
            else {
                return fallbackTable[(int)fallbackPriority][(int)fallbackRespect][id];
            }
        }

        void FindFallback(ref int id) {
            int counter = 0;
            while (id != 0) {
                if (tiles[id].sprite == null) {
                    id = GetFallback(id);
                }
                else {
                    break;
                }
                counter++;
                if (counter > 10) {
                    Debug.Log("Broken Tile: " + id.ToString());
                    break;
                }
            }
        }

        int GetSpriteTile(Sprite spr, Matrix4x4 transform) {
            int len = tiles.Length;
            for (int i = 0; i < len; i++) {
                if (tiles[i].transform != transform) continue;

                // Standard sprite
                if (tiles[i].sprite == spr) {
                    return i;
                }

                // Variations
                if (tiles[i].variations != null) {
                    for (int v = 0; v < tiles[i].variations.Count; v++) {
                        if (tiles[i].variations[v].sprite == spr) {
                            return i;
                        }
                    }
                }
            }
            return -1;
        }

        int GetSurroundings(Vector3Int location, ITilemap tileMap) {
            int around = 0;
            int pos = 0;
            for (int j = 1; j >= -1; j--) {
                for (int i = -1; i <= 1; i++) {
                    // Skip center tile (myself)
                    if (i == 0 && j == 0) continue;

                    // Check tile
                    var target = location + new Vector3Int(i, j, 0);
                    if (HasTileAt(tileMap, target)) {
                        around += (byte)Mathf.Pow(2, pos);
                    }

                    // Next
                    pos++;
                }
            }
            return around;
        }

        public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData) {
            // Get surroundings
            int around = GetSurroundings(location, tileMap);

            // Force tiles to separete fully when not implemented
            if (renderStage == RenderStage.Force) {
                if (forceHorizontal) {
                    // Left
                    if ((around & 0b0000_1000) > 0) {
                        var target = location + new Vector3Int(-1, 0, 0);
                        int id = GetSpriteTile(tileMap.GetSprite(target), Matrix4x4.identity);

                        if (tiles[id].sprite == null || true) {
                            FindFallback(ref id);
                            int force = forceTable[id] / 10;
                            if (force == 2 || force == 3) around = around & 0b1111_0111;
                        }
                    }

                    // Right
                    if ((around & 0b0001_0000) > 0) {
                        var target = location + new Vector3Int(1, 0, 0);
                        int id = GetSpriteTile(tileMap.GetSprite(target), Matrix4x4.identity);

                        if (tiles[id].sprite == null || true) {
                            FindFallback(ref id);
                            int force = forceTable[id] / 10;
                            if (force == 1 || force == 3) around = around & 0b1110_1111;
                        }
                    }
                }
                if (forceVertical) {
                    // Top
                    if ((around & 0b0000_0010) > 0) {
                        var target = location + new Vector3Int(0, 1, 0);
                        int id = GetSpriteTile(tileMap.GetSprite(target), Matrix4x4.identity);

                        if (tiles[id].sprite == null || true) {
                            FindFallback(ref id);
                            int force = forceTable[id] % 10;
                            if (force == 2 || force == 3) around = around & 0b1111_1101;
                        }
                    }

                    // Bottom
                    if ((around & 0b0100_0000) > 0) {
                        var target = location + new Vector3Int(0, -1, 0);
                        int id = GetSpriteTile(tileMap.GetSprite(target), Matrix4x4.identity);

                        if (tiles[id].sprite == null || true) {
                            FindFallback(ref id);
                            int force = forceTable[id] % 10;
                            if (force == 1 || force == 3) around = around & 0b1011_1111;
                        }
                    }
                }
            }

            // Update tile
            UpdateTile(location, tileMap, ref tileData, around);
        }

        bool HasTileAt(ITilemap tileMap, Vector3Int position) {
            // Check whether the tile is ours
            TileBase tile = tileMap.GetTile(position);

            // Ignore null tiles
            if (tile == null) return false;

            // Check other allowed tiles
            if (tile != this) {
                int len = allowedTiles.Count;
                for (int i = 0; i < len; i++) {
                    if (tile == allowedTiles[i]) return true;
                }
            }

            return tile == this;
        }

        void UpdateTile(Vector3Int location, ITilemap tileMap, ref TileData tileData, int around = 0) {
            if (!HasTileAt(tileMap, location)) return;

            // Decide tile
            int id = tileTable[around];

            // Fallbacks
            FindFallback(ref id);

            // Apply
            var data = tiles[id];
            tileData.sprite = data.sprite;
            tileData.transform = data.transform;
            tileData.color = baseColor;
            tileData.flags = TileFlags.LockAll;
            if (data.collider == ColliderType.Standard) tileData.colliderType = colliderType;
            else tileData.colliderType = (Tile.ColliderType)((int)data.collider - 1);

            // Sprite variations
            if (data.variations != null) {
                int len = data.variations.Count;
                float chance = 0, rnd = Random.value;
                for (int i = 0; i < len; i++) {
                    chance += data.variations[i].chance / 100f;
                    if (rnd < chance) {
                        tileData.sprite = data.variations[i].sprite;
                        break;
                    }
                }
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(OmniTerrain))]
    public class OmniTerrainEditor : UnityEditor.Editor {
        OmniTerrain tile { get { return (target as OmniTerrain); } }
        GUIStyle styleHeaderLabel;
        bool tilesetExpanded = true;

        public void OnEnable() {
            // Create tile array (expand if needed)
            int len = System.Enum.GetNames(typeof(OmniTerrain.TileID)).Length;
            if (tile.tiles == null || tile.tiles.Length != len) {
                // Create array
                var temp = new OmniTerrain.TileProperties[len];
                for (int i = 0; i < len; i++) {
                    temp[i] = new OmniTerrain.TileProperties();
                }

                // Copy former elements
                tile.tiles?.CopyTo(temp, 0);
                tile.tiles = temp;

                // Modified
                EditorUtility.SetDirty(tile);
            }

            // Style
            styleHeaderLabel = new GUIStyle {
                fontStyle = FontStyle.BoldAndItalic,
                fontSize = 16
            };
            styleHeaderLabel.normal.textColor = Color.white;
        }

        void GUIAllowedTiles() {
            // Create
            if (tile.allowedTiles == null) tile.allowedTiles = new List<TileBase>();

            // Update length
            int curlen = tile.allowedTiles.Count;
            int len = EditorGUILayout.IntField("Allowed Tiles", curlen);

            // Resize needed?
            while (len != curlen) {
                if (len > curlen) {
                    tile.allowedTiles.Add(null);
                }
                else {
                    tile.allowedTiles.RemoveAt(curlen - 1);
                }
                curlen = tile.allowedTiles.Count;
            }

            // Display elements
            for (int i = 0; i < len; i++) {
                tile.allowedTiles[i] = (TileBase)EditorGUILayout.ObjectField(string.Format("    Tile {0}", i), tile.allowedTiles[i],
                    typeof(TileBase), false);
            }
        }

        void GUIVariations(OmniTerrain.TileProperties t) {
            int curlen = t.variations == null ? 0 : t.variations.Count;

            // Shrink/expand tileset
            string label = t.variationsExpanded ? "    Variations" : string.Format("    Variations ({0})", curlen);
            t.variationsExpanded = EditorGUILayout.Foldout(t.variationsExpanded, label);
            if (!t.variationsExpanded) return;

            // Length field
            int len = EditorGUILayout.IntField("        Length", curlen);

            // Changed?
            if (len != curlen) {
                if (len > 0) {
                    // Create list
                    if (t.variations == null) t.variations = new List<OmniTerrain.TileVariation>();

                    // Resize list
                    while (len != curlen) {
                        if (len > curlen) {
                            t.variations.Add(new OmniTerrain.TileVariation());
                        }
                        else {
                            t.variations.RemoveAt(curlen - 1);
                        }
                        curlen = t.variations.Count;
                    }
                }
                else {
                    // Zero length: erase list
                    t.variations = null;
                }
            }

            // Display elements
            if (len > 0) {
                float total = 0f;
                for (int i = 0; i < len; i++) {
                    EditorGUILayout.LabelField(string.Format("        Alternative {0}", i + 1));
                    t.variations[i].sprite = (Sprite)EditorGUILayout.ObjectField("            Sprite ",
                        t.variations[i].sprite, typeof(Sprite), false);
                    t.variations[i].chance = EditorGUILayout.FloatField("            Chance (%)", t.variations[i].chance);
                    total += t.variations[i].chance;
                }
                if (total <= 100f) {
                    EditorGUILayout.LabelField("        Default Tile Chance", string.Format("{0}%", 100 - total));
                }
                else {
                    EditorGUILayout.LabelField(string.Format("Total chance must be lower than 100%! (current: {0}%)", total));
                }
            }
        }

        public override void OnInspectorGUI() {
            // Setup
            float oldw = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 180;
            EditorGUI.BeginChangeCheck();

            // Global properties
            tile.colliderType = (Tile.ColliderType)EditorGUILayout.EnumPopup("Collider Type", tile.colliderType);
            tile.baseColor = EditorGUILayout.ColorField("Base Color", tile.baseColor);

            // Allowed tiles
            GUIAllowedTiles();

            // Fallbacks
            EditorGUILayout.Space();
            var priority = (OmniTerrain.FallbackPriority)EditorGUILayout.EnumPopup("Fallback Priority", tile.fallbackPriority);
            var respect = tile.fallbackRespect;
            var force = tile.fallbackForce;
            if (priority != OmniTerrain.FallbackPriority.Custom) {
                respect = (OmniTerrain.FallbackRespect)EditorGUILayout.EnumPopup("Respect Corners", tile.fallbackRespect);
                if (respect == OmniTerrain.FallbackRespect.Both || true) {
                    force = (OmniTerrain.FallbackForce)EditorGUILayout.EnumPopup("Force Corners", tile.fallbackForce);

                    // Disable force: not working
                    force = OmniTerrain.FallbackForce.None;
                }
            }
            EditorGUILayout.Space();

            // Priority modes
            if (priority != tile.fallbackPriority) {
                if (priority == OmniTerrain.FallbackPriority.Custom) {
                    // Set defaults
                    int len = tile.tiles.Length;
                    for (int i = 0; i < len; i++) {
                        tile.tiles[i].fallback = tile.GetFallback(i);
                    }

                    // Disable force
                    force = OmniTerrain.FallbackForce.None;
                }
                tile.fallbackPriority = priority;
            }

            // Respect modes
            if (respect != tile.fallbackRespect) {
                /*if (respect != OmniTerrain.FallbackRespect.Both) {
                    force = OmniTerrain.FallbackForce.None;
                }*/
                tile.fallbackRespect = respect;
            }

            // Force modes
            if (force != tile.fallbackForce) {
                switch (force) {
                    case OmniTerrain.FallbackForce.None:
                        tile.forceHorizontal = false;
                        tile.forceVertical = false;
                        break;
                    case OmniTerrain.FallbackForce.Horizontal:
                        tile.forceHorizontal = true;
                        tile.forceVertical = false;
                        break;
                    case OmniTerrain.FallbackForce.Vertical:
                        tile.forceHorizontal = false;
                        tile.forceVertical = true;
                        break;
                    case OmniTerrain.FallbackForce.Both:
                        tile.forceHorizontal = true;
                        tile.forceVertical = true;
                        break;
                }
                tile.fallbackForce = force;
            }

            // Shrink/expand tileset
            tilesetExpanded = EditorGUILayout.Foldout(tilesetExpanded, "Tileset");

            // Tiles
            if (tilesetExpanded) {
                int len = tile.tiles.Length;
                for (int i = 0; i < len; i++) {
                    var t = tile.tiles[i];

                    // Title
                    string name = string.Format("{0:D2}: {1}", i, (OmniTerrain.TileID)i);
                    EditorGUILayout.LabelField(name, styleHeaderLabel);

                    // Parameters
                    t.sprite = (Sprite)EditorGUILayout.ObjectField("", t.sprite, typeof(Sprite), false);
                    var distortion = (OmniTerrain.TileDistortion)EditorGUILayout.EnumPopup("    Transformations", t.distortion);
                    t.collider = (OmniTerrain.ColliderType)EditorGUILayout.EnumPopup("    Collider Type", t.collider);
                    if (tile.fallbackPriority == OmniTerrain.FallbackPriority.Custom) {
                        t.fallback = EditorGUILayout.IntField("    Fallback", t.fallback);
                    }

                    // Transformations
                    if (distortion != t.distortion) {
                        switch (distortion) {
                            case OmniTerrain.TileDistortion.None:
                                t.transform = Matrix4x4.identity;
                                break;
                            case OmniTerrain.TileDistortion.FlipHorizontal:
                                t.transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(-1f, 1f, 1f));
                                break;
                            case OmniTerrain.TileDistortion.FlipVertical:
                                t.transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f, -1f, 1f));
                                break;
                            case OmniTerrain.TileDistortion.RotateCW:
                                t.transform = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -90f), Vector3.one);
                                break;
                            case OmniTerrain.TileDistortion.RotateCCW:
                                t.transform = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 90f), Vector3.one);
                                break;
                            case OmniTerrain.TileDistortion.Rotate180:
                                t.transform = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 180f), Vector3.one);
                                break;
                        }
                        t.distortion = distortion;
                    }

                    // Variations
                    GUIVariations(t);

                    // Space
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    // Division: optional tiles after 22
                    if (i == 22) {
                        EditorGUILayout.LabelField("----- OPTIONAL TILES -----", styleHeaderLabel);
                        EditorGUILayout.Space();
                        EditorGUILayout.Space();
                    }
                }
            }

            // Finish
            if (EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(tile);
            }
            EditorGUIUtility.labelWidth = oldw;
        }
    }
#endif
}