/// <summary>
/// This is obviously incomplete - not 100% sure how this will work yet
/// There will need to be a lot of state info about the level originating in this manager, fed to scene objects
/// 
/// Option 1. could have an individual scene manager for every level, but that will end up being a lot of repeated code.
/// Option 2. we could start another class hierarchy here (LevelOneManager : BaseLevelManager : BaseSceneManager) that can handle multiple scenes, which would cut down on most repeated code
/// 
/// Have to weigh pros and cons.
/// </summary>
public class Level1Manager : BaseSceneManager
{
    public override string ManagedSceneName => "Level1";

    public override GameState ManagedState => GameState.Level1;
}
