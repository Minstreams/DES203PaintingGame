using UnityEngine;

namespace GameSystem.Operator
{
    [AddComponentMenu("[GameFlowSystem]/Operator/GameMessageSender")]
    public class GameMessageSender : MonoBehaviour
    {
        [MinsHeader("Operator of GameFlowSystem", SummaryType.PreTitleOperator, -1)]
        [MinsHeader("Game Message Sender", SummaryType.TitleOrange, 0)]
        [MinsHeader("Use this to send a game message to GameFlowSystem", SummaryType.CommentCenter, 1)]
        [MinsHeader("Data", SummaryType.Header, 2)]
        [Label] public GameMessage message;
        [Label(true)] public bool sendOnStart;

        // Inner code here
        void Start()
        {
            if (sendOnStart) SendGameMessage();
        }

        // Input
        [ContextMenu("Send")]
        public void SendGameMessage()
        {
            GameFlowSystem.SendGameMessage(message);
        }
    }
}