using System;
using System.Collections.Generic;


namespace Framework
{
    [Serializable]
    public class SequentialSelectorData : ISelectorData
    {

    }
    public sealed class SequentialSelector : UtilityBase<SequentialSelectorData>, IUtilitySelector
    {
        int _count;
        int _curIdx;

        public int Select(List<UtilityValue> elements)
        {
            var count = elements.Count;
            if (count == 0)
                return -1;

            if (_count != count)
            {
                _curIdx = 0;
                _count = count;
                return _curIdx;
            }

            _curIdx++;
            _curIdx = _curIdx % _count;
            return _curIdx;
        }

        protected override void OnInit(SequentialSelectorData data)
        {
        }
    }

}