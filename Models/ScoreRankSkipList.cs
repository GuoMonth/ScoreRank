namespace ScoreRank.Models
{
    /// <summary>
    /// 跳表数据结构，用于高效维护客户排名
    /// </summary>
    public class ScoreRankSkipList
    {
        /// <summary>
        /// 跳表的最大层数, 32层足以支持百万级数据的高效操作
        /// </summary>
        private const int MaxLevel = 32; // 最大层数

        /// <summary>
        /// 跳表的头节点
        /// </summary>
        private readonly SkipListNode _head;

        /// <summary>
        /// 当前跳表的最高层数
        /// </summary>
        private int _currentLevel;

        /// <summary>
        /// 随机数生成器
        /// </summary>
        private readonly Random _rng = new();

        /// <summary>
        /// 索引字典，用于快速定位客户节点
        /// </summary>
        private readonly Dictionary<long, SkipListNode> _index = new();

        /// <summary>
        /// 跳表中节点的数量
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// 初始化跳表
        /// </summary>
        public ScoreRankSkipList()
        {
            _head = new SkipListNode(MaxLevel, new CustomerRank { CustomerId = long.MinValue });
            _currentLevel = 0;
        }

        /// <summary>
        /// 随机选择节点的层数, 概率为1/4的几何分布
        /// </summary>
        /// <returns></returns>
        private int PickRandomLevel()
        {
            int level = 0;

            // (_rng.Next() & 3) == 0, 25%的概率增加一层.
            // 具体来说, 3的二进制是11, 因此 00, 01, 10 三种结果不会增加层数, 
            // 只有11会增加层数, 概率为1/4.
            while (level < MaxLevel && (_rng.Next() & 3) == 0)
            {
                level++;
            }
            return level;
        }


        /// <summary>
        /// 获取指定客户的排名
        /// </summary>
        /// <param name="customerId">客户ID</param>
        public int GetRank(long customerId)
        {
            if (_index.TryGetValue(customerId, out var rankNode) == false)
            {
                return -1; // 客户不存在
            }

            int rank = 0;
            var currentNode = _head;

            // 计算排名
            for (int level = _currentLevel; level >= 0; level--)
            {
                while (currentNode.Forward[level] != null &&
                       (currentNode.Forward[level].Data.Score > rankNode.Data.Score ||
                        (currentNode.Forward[level].Data.Score == rankNode.Data.Score &&
                         currentNode.Forward[level].Data.CustomerId < customerId)))
                {
                    rank += currentNode.Span[level];
                    currentNode = currentNode.Forward[level];
                }
            }

            return rank + 1; // 排名从1开始
        }
    }
}