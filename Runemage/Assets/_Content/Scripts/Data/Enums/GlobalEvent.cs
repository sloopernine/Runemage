namespace Data.Enums
{
    public enum GlobalEvent
    {
        // GAME STATE EVENT
        WIN_GAMESTATE,
        LOST_GAMESTATE,
        PAUSED_GAMESTATE,
        PLAY_GAMESTATE,
        
        PLAY_SOUND,
        OBJECT_INACTIVE,

        // DEBUG
        DEBUG_ON,
        DEBUG_OFF,
        SHOW_FPS,
        HIDE_FPS,
        SHIELD_INVULNERABLE_ON,
        SHIELD_INVULNERABLE_OFF,
        SHIELD_RESET,
        SET_NEXT_ROUND,
        SET_ROUND,
        SPELLS_DESTROY_ALL,

        // RUNE CLOUD
        RUNECLOUD_DESTROYED,
        
		// RUNE MAKING
		CREATE_SPELL,

        // Enemies
        ENEMY_DESTROY_ALL
    }
}