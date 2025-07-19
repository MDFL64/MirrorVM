using MirrorVM;
using System;
using System.Diagnostics;

public class BenchRunner
{
	static string[] Benchmarks = ["hashes", "image", "json", "prospero_compile", "prospero_eval", "rand_sort", "rapier", "regex", "zip"];

	public static async void DoBenchmarks()
	{
		var file = FileSystem.Mounted.OpenRead( "rust_bench.wasm" );

		var module = new MirrorVM.WasmModule( file, null );
		var instance = new MirrorVM.WasmInstance( module );
		var frame = new MirrorVM.Frame( 1 );

		string results = "";

		foreach ( var bench_name in Benchmarks )
		{
			if ( module.Exports.TryGetValue( "bench_" + bench_name, out object item ) )
			{
				var func = item as MirrorVM.WasmFunction;
				if ( func != null )
				{
					Log.Info( bench_name );

					var callable = func.GetBody().Compile();

					List<TimeSpan> times = [];

					for ( int i = 0; i < 10; i++ )
					{
						var start = Stopwatch.StartNew();
						callable.Call( frame, instance );
						times.Add( start.Elapsed );
						Log.Info( i );
						await GameTask.Delay( 100 );
					}

					times.Sort();
					Log.Info( "min = " + times[0] );
					Log.Info( "max = " + times[times.Count - 1] );

					results += bench_name + "," + times[0].TotalSeconds + "\n";
				}
			}
		}

		Log.Info( results );
	}

	/*private async void DoWasmBox()
	{
		var file = FileSystem.Mounted.OpenRead( "rust_bench.wasm" );

		var module = WasmBox.Wasm.WasmFile.ReadBinary( file );
		var instance = ModuleInstance.Instantiate( module );

		string results = "";

		foreach ( var bench_name in Benchmarks )
		{
			if ( instance.ExportedFunctions.TryGetValue( "bench_" + bench_name, out var func ) )
			{
				Log.Info( bench_name );
				var start = Stopwatch.StartNew();
				func.Invoke( [] );
				var time = start.Elapsed;
				Log.Info( time );
				await GameTask.Delay( 100 );

				results += bench_name + "," + time.TotalSeconds + "\n";
			}
		}

		Log.Info( results );
	}

	private async void DoDextr()
	{
		var mod = new Dextr.Module();

		List<TimeSpan> times = [];

		for ( int i = 0; i < 10; i++ )
		{
			var start = Stopwatch.StartNew();
			var res = mod.bench_json();
			Log.Info( "> " + i + " " + res );
			times.Add( start.Elapsed );
			await GameTask.Delay( 100 );
		}

		times.Sort();
		Log.Info( "min = " + times[0] );
		Log.Info( "max = " + times[times.Count - 1] );
	}*/
}
