namespace Data.Enums
{
    public enum GlobalEvent
    {
        // GAME STATE EVENT
        WIN_GAME,
        PAUSE_GAME,
        UNPAUSE_GAME,
        
        PLAY_SOUND,
        OBJECT_INACTIVE,

        // DEBUG
        DEBUG_ON,
        DEBUG_OFF,
        SHOW_FPS,
        HIDE_FPS,
        SHIELD_INVULNERABLE_ON,
        SHIELD_INVULNERABLE_OFF,
        SET_NEXT_ROUND,
        SET_ROUND,

        // RUNE CLOUD
        RUNECLOUD_DESTROYED,
        
		// RUNE MAKING
		CREATE_SPELL_ORIGIN,

        // Enemies
        ENEMY_DESTROY_ALL,



    }
}