using UnityEngine;

public static class Const {
    public static int PLAYER_NUMBER = 2;

    public static int CHAR_NUMBER = 4 * PLAYER_NUMBER;
    public static int BOARD_ROWS = 5;
    public static int BOARD_COLS = 5;

    public static int SKILL_NUMBER = 4;
    public static int MAX_ENERGY = 10;

    // Offset over Y-Axis of the beam of light
    public static float BEAMLIGHT_OFFSET = 3f;
    // Offset over Y-Axis of characters
    public static float CHAR_OFFSET = 0.05f;
    // Offset over Y-Axis of projectiles orientation
    public static float PROJ_OFFSET = 0.7f;
    // Time that characters animations wait at beginning of use skill for dice roll
    public static float DICE_ROLL_TIME = 1f;
    // Time that characters animations wait after last of them finish its skill/reaction animation to return idle position
    public static float WAIT_AFTER_SKILL_ANIM = 1.5f;
    // Time spend on fade out effect when a character dead
    public static float CHAR_FADE_OUT_SECONDS = 2f;
    // Time spend on fade out effect when a projectile impact (if needed)
    public static float PROJ_FADE_OUT_SECONDS = 5f;
    // Animation time for character stumbling when was shoved
    public static float SHOVE_ANIM_TIME = 0.8f;
    // Time server wait before apply collision damage due to shove (needed to sync damage with animation)
    public static float SHOVE_COLLISION_TIME = 2f;
    // Damage caused to characters when collide after a shove
    public static int SHOVE_COLLISION_DAMAGE = 1;

    // Animation time to resize portraits when finish turn
    public static float PORTRAIT_ENDTURN_TIME = 0.25f;
    // Animation time to relocate and resize portraits when skip turn
    public static float PORTRAIT_SKIPTURN_TIME = 0.5f;
    // Animation time to relocate and resize portraits when a new round starts
    public static float PORTRAIT_STARTROUND_TIME = 0.5f;
    // Animation time to remove and resize portraits when a character dead
    public static float PORTRAIT_DEATH_TIME = 0.9f;
    // Distance between adjacent portraits
    public static float PORTRAIT_DISTANCE = 100f;
    // Distance increase/decrease between two portraits when one of them is selected/unselected (due to resize)
    public static float PORTRAIT_RESIZE_OFFSET = 24f;
    // Portrait size when character is selected
    public static float PORTRAIT_SELECT_SIZE = 1.5f;
    // Portrait size when character is unselected
    public static float PORTRAIT_UNSELECT_SIZE = 1f;

    #region Colors
    //mr.material.color = new Color32(255, 255, 255, 255);
    //mr.material.color = new Color32(178, 178, 178, 255);
    public static Color32 TILE_DEFAULT = new Color32(154, 154, 154, 255);
    public static Color32 TILE_GREEN = new Color32(125, 200, 125, 255);
    public static Color32 TILE_RED = new Color32(200, 125, 125, 255);

    //public static Color32 COLOR_PLAYER1 = new Color(255, 0, 0);
    //public static Color32 COLOR_PLAYER1 = new Color32(255, 30, 30, 100);
    public static Color32 COLOR_PLAYER1 = new Color32(255, 30, 30, 150);
    //public static Color32 COLOR_PLAYER1 = new Color32(120, 15, 15, 150);
    //public static Color32 COLOR_PLAYER2 = new Color(0, 125, 255);
    //public static Color32 COLOR_PLAYER2 = new Color32(0, 180, 255, 100);
    //public static Color32 COLOR_PLAYER2 = new Color32(0, 255, 255, 150);
    public static Color32 COLOR_PLAYER2 = new Color32(0, 100, 255, 150);
    //public static Color32 COLOR_PLAYER2 = new Color32(0, 44, 120, 150);

    //public static Color32 RED_CRYSTAL = new Color32(210, 20, 20, 255);
    public static Color32 RED_CRYSTAL = new Color32(255, 60, 60, 255);
    //public static Color32 BLUE_CRYSTAL = new Color32(20, 20, 210, 255);
    public static Color32 BLUE_CRYSTAL = new Color32(100, 110, 255, 255);
    #endregion
}
