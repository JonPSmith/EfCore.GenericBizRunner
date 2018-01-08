// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;

namespace TestBizLayer.BizDTOs
{
    [Flags]
    public enum ActionModes
    {
        AllOk, FirstError = 1, FirstWarn = 2, SecondError = 4, SecondWarn = 8, FinalError = 16, FinalWarn = 32,
        FirstWriteBad = 128, SecondWriteBad = 256
    }

    public class BizDataGuid
    {
        public BizDataGuid(ActionModes modes)
        {
            Modes = modes;
            Unique = Guid.NewGuid();
        }

        public ActionModes Modes { get; set; }

        public Guid Unique { get; set; }

        public bool ShouldError(int stageNum)
        {
            var names = Modes.ToString();
            switch (stageNum)
            {
                case 1:
                    return names.Contains("FirstError");
                case 2:
                    return names.Contains("SecondError");
                case 3:
                    return names.Contains("FinalError");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool ShouldWarn(int stageNum)
        {
            var names = Modes.ToString();
            switch (stageNum)
            {
                case 1:
                    return names.Contains("FirstWarn");
                case 2:
                    return names.Contains("SecondWarn");
                case 3:
                    return names.Contains("FinalWarn");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}