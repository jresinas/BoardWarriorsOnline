using UnityEngine;

public static class Const {
    public static int PLAYER_NUMBER = 2;

    public static int CHAR_NUMBER = 4 * PLAYER_NUMBER;
    public static int BOARD_ROWS = 5;
    public static int BOARD_COLS = 5;

    public static int SKILL_NUMBER = 4;
    public static int MAX_ENERGY = 10;

    public static float CHAR_OFFSET = 0.05f;
    public static float DICE_ROLL_TIME = 1f;
    public static float WAIT_AFTER_SKILL_ANIM = 1.5f;
    public static float CHAR_FADE_OUT_SECONDS = 2f;
    public static float PROJ_FADE_OUT_SECONDS = 5f;
    public static float SHOVE_ANIM_TIME = 0.8f;

    public static float PORTRAIT_ENDTURN_TIME = 0.25f;
    public static float PORTRAIT_SKIPTURN_TIME = 0.5f;
    public static float PORTRAIT_DEATH_TIME = 0.9f;
    public static float PORTRAIT_DISTANCE = 100f;
    public static float PORTRAIT_RESIZE_OFFSET = 24f;
    public static float PORTRAIT_SELECT_RESIZE = 1.5f;

    #region Colors
    //mr.material.color = new Color32(255, 255, 255, 255);
    //mr.material.color = new Color32(178, 178, 178, 255);
    public static Color32 TILE_DEFAULT = new Color32(154, 154, 154, 255);
    public static Color32 TILE_GREEN = new Color32(125, 200, 125, 255);
    public static Color32 TILE_RED = new Color32(200, 125, 125, 255);
    #endregion
}
