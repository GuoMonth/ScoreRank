namespace ScoreRank.Models
{
    /// <summary>
    /// 核心对象: 跳表的节点
    /// </summary>
    public class SkipListNode
    {
        /// <summary>
        /// 节点存储的数据
        /// </summary>
        public CustomerRank Data { get; set; }

        /// <summary>
        /// 节点在各级索引中的前进指针数组
        /// </summary>
        public SkipListNode[] Forward { get; set; }

        /// <summary>
        /// 节点在各级索引中的跨度数组
        /// </summary>
        public int[] Span { get; }

        /// <summary>
        /// 初始化跳表节点
        /// </summary>
        /// <param name="level"></param>
        /// <param name="data"></param>
        public SkipListNode(int level, CustomerRank data)
        {
            Data = data;
            Forward = new SkipListNode[level + 1];
            Span = new int[level + 1];
        }
    }
}