using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyaku
{
    public class GameOverEventArgs
    {
        public GameOverReason Reason
        {
            get;
            set;
        }

        public GameOverEventArgs(GameOverReason reason)
        {
            Reason = reason;
        }
    }

    public enum GameOverReason
    {
        PushedPastTop,
        RanOutOfSpace,
        Error
    }
}
