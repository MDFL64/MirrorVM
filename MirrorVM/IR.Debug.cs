namespace MirrorVM
{
	class DebugIR
	{
		public static void Dump(Block init, string name, bool draw_backlinks)
		{
			if (!Config.DUMP_IR)
			{
				return;
			}

			if (name != null)
			{
				name = name.Replace('$', '_');
				const int MAX_LENGTH = 64;
				if (name.Length > MAX_LENGTH)
				{
					name = name.Substring(0, MAX_LENGTH);
				}
			}

			//return;
			HashSet<Block> Closed = new HashSet<Block>();
			Queue<Block> Open = new Queue<Block>();
			Open.Enqueue(init);
			Closed.Add(init);

			// theme stolen from https://cprimozic.net/notes/posts/basic-graphviz-dark-theme-config/
			string result = """
        digraph {
            bgcolor="#181818";

            node [
                fontname = "Consolas";
                fontcolor = "#e6e6e6",
                style = filled,
                color = "#e6e6e6",
                fillcolor = "#333333"
            ]

            edge [
                fontname = "Arial";
                color = "#e6e6e6",
                fontcolor = "#e6e6e6"
            ]
        """;

			while (Open.Count > 0)
			{
				var block = Open.Dequeue();
				string block_str = DumpBlock(block).Replace("\n", "\\l");
				//Console.WriteLine("==> "+block_str);
				result += "\t" + block.Name + " [ shape=box label =\"" + block_str + "\" ]\n";

				if (block.Terminator != null)
				{
					var next_blocks = block.Terminator.GetNextBlocks();
					for (int i = 0; i < next_blocks.Count; i++)
					{
						// add link
						var next = next_blocks[i];
						var label = block.Terminator.LabelLink(i);
						result += "\t" + block.Name + " -> " + next.Name + " [label = \"" + label + "\"]\n";

						// enqueue block
						if (!Closed.Contains(next))
						{
							Open.Enqueue(next);
							Closed.Add(next);
						}
					}
				}
				else
				{
					result += "\t" + block.Name + " -> ERROR [color=red constraint=false]\n";
				}
				if (draw_backlinks)
				{
					foreach (var pred in block.Predecessors)
					{
						result += "\t" + block.Name + " -> " + pred.Name + " [color=yellow constraint=false]\n";
					}
				}
			}
			result += "}";
#if SANDBOX
			Log.Info( "Saved IR Dump: " + name );
			FileSystem.Data.WriteAllText(name + ".dot", result);
#else
			Console.WriteLine("Saved IR Dump: " + name);
			File.WriteAllText( "graph/" + name + ".dot", result );
#endif
		}

		public static string DumpBlock( Block b )
		{
			string res = "#" + b.Index + " (cost = " + b.Cost + ")\n" + DumpStatements( 0, b.Statements );
			if ( b.Terminator == null )
			{
				res += "ERROR: NO TERMINATOR!";
			}
			else
			{
				res += b.Terminator;
			}
			return res + "\n";
		}

		public static string DumpStatements( int depth, List<(Destination?, Expression)> stmts )
		{
			string tabs = Tabs( depth );

			string res = "";
			foreach ( var stmt in stmts )
			{
				(var dst, var src) = stmt;
				if ( dst != null )
				{
					res += tabs + dst + " = " + src + "\n";
				}
				else if ( src is ControlStatement ctrl )
				{
					res += tabs + ctrl.ToString( depth );
				}
				else
				{
					res += tabs + src + "\n";
				}
			}
			return res;
		}

		public static string Tabs( int depth )
		{
			return string.Concat( Enumerable.Repeat( "  ", depth ) );
		}
	}
}
