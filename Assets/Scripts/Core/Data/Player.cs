using Core.Game;
using Core.Items;
using Core.Moves;

namespace Core.Data
{
    public class Player
    {
        public PlayerState PlayerState { get; }

        public long NumMoves { get; private set; }
        public bool Win => PlayerState.IsFinal;

        public Player(GameBoard gameBoard)
        {
            PlayerState = new PlayerState(gameBoard);
            MoveTo(gameBoard.StartNode);

            NumMoves += gameBoard.Metadata.Moves;
        }

        public void MoveTo(Node node)
        {
            PlayerState.MoveTo(node);
        }

        public bool IsProximal(Field field)
        {
            return PlayerState.Contains(field.ParentNode) || PlayerState.Contains(field.ConnectedNode);
        }

        public bool PlayMove(IMove move)
        {
            var movePlayed = move.Play();
            
            if (movePlayed && move is PushMove) {
                NumMoves++;
            }

            return movePlayed;
        }
    }
}
