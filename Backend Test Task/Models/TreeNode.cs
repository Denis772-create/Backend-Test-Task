namespace Backend_Test_Task.Models
{
    public class TreeNode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public TreeNode? Parent { get; set; }
        public ICollection<TreeNode>? Children { get; set; }
        public int TreeId { get; set; }
        public Tree Tree { get; set; }
    }
}
