using System.Collections;
using System.Collections.Generic;

namespace GoingMedievalModLauncher.plugins
{

	public class PluginTree : IEnumerable<PluginTree.TreeNode>
	{
		
		private List<TreeNode> _leaves = new List<TreeNode>(32);
		public IEnumerable<TreeNode> leaves => _leaves;
		public readonly RootNode root = new RootNode(ModLoaderPluginContainer.Instance);

		public PluginTree()
		{
			_leaves.Add(root);
		}
		
		public void Add(TreeNode item)
		{
			if ( _leaves.Contains(item.parent) )
			{
				_leaves.Remove(item.parent);
			}
			_leaves.Add(item);
			item.parent.AddChild(item);
		}

		public void Remove(TreeNode node)
		{
			foreach ( var cnode in this )
			{
				if ( node == cnode )
				{
					cnode.parent.RemoveChild(cnode);
				}
			}
		}

		public bool Validate()
		{
			var valid = true;

			foreach ( var leaf in _leaves )
			{
				var curr = leaf;
				while ( curr.parent != null )
				{
					curr = curr.parent;
				}

				valid = curr == root;
				if(!valid)
					break;
			}

			return valid;
		}

		private IEnumerable<TreeNode> Traverse => root.traverse();

		public IEnumerator<TreeNode> GetEnumerator()
		{
			yield return root;
			foreach ( var node in Traverse ) yield return node;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


		public class TreeNode
		{

			private TreeNode _parent;
			public TreeNode parent => _parent;
			private readonly List<TreeNode> _children = new List<TreeNode>();
			public IEnumerable<TreeNode> children => _children;

			public readonly IPluginContainer value;

			public void AddChild(TreeNode child)
			{
				_children.Add(child);
			}

			public void RemoveChild(TreeNode child)
			{
				if ( _children.Contains(child) )
				{
					_children.Remove(child);
					child._parent = null;
				}
			}

			public TreeNode(TreeNode parent, IPluginContainer value)
			{
				_parent = parent;
				this.value = value;
			}

			public IEnumerable<TreeNode> traverse()
			{
				foreach ( var node in children )
				{
					yield return node;
				}

				foreach ( var node in children )
				{
					foreach ( var treeNode in node.traverse() )
					{
						yield return treeNode;
					}
				}

			}

		}

		public sealed class RootNode : TreeNode
		{

			public RootNode(IPluginContainer value) : base(null, value){}

		}

	}
}