namespace Backend_Test_Task.Models
{
    public class Tree
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<TreeNode>? Nodes { get; set; }
    }
}
