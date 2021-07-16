// COLLECTION OF ENUMS

public enum Worlds {
    W1,
    W2,
    W3    
}

public enum Levels {
    //WorldSelection,
    //MainMenu,

    //World 1
    W1L1,
    W1L1Q,
    W1L2,
    W1L2Q,
    W1L3,
    W1L3Q,
    W1L4,
    W1L4Q,
    W1L5,
    W1L5Q

    //World 2
    /*
    W2L1,
    W2L1Q,
    W2L2,
    W2L2Q,
    W2L3,
    W2L3Q,
    W2L4,
    W2L4Q,
    W2L5,
    W2L5Q
    */
}

public enum MenuScene {
    MainMenu,
    WorldSelection
}

public enum PowerUp {
    None,
    Shuffle,
    Pick,
    Chamma,
    Dynamite
}

//This is used to solve the issue where we could play while the game is still filling/refilling the grid//
public enum GameState {
    wait,
    move
}

public enum TileKind {
    Breakable,
    FrozenRock,
    Blank,
    Normal
}

public enum MissionType {
    SpecialItem,
    FrozenRock,
    Breakable

}