public struct GameStates 
{
    public enum State { Idle, Login, WelcomingDialogue,ShelvesFilling, 
        ShelvesRotate, EmojisDisplayNames,Tutorial, UserTest,Results,ShowPerformance, BenefitsIntro,BenefitsMenu, BenefitsShowcase   }
    public static State GetStateByIndex(int index) => (State)index;
}
