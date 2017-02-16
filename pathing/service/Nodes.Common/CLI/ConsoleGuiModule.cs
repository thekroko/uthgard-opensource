using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ninject;
using Nodes.Tasks;
using Timer = System.Timers.Timer;

namespace Nodes.CLI {
  /// <summary>
  ///   A Console stats GUI that refreshes itself
  /// </summary>
  public class ConsoleGuiModule : AbstractTaskModule {
    /// <summary>
    ///   Called when a new console frame should be rendered
    /// </summary>
    /// <param name="helper">Helpers for drawing UIs</param>
    public delegate void RenderFrame(GuiHelper helper);

    /// <summary>
    ///   Interval at which the GUI is refreshed
    /// </summary>
    public static int UpdateInterval = 1000;

    private readonly GuiHelper helper = new GuiHelper();

    private int frame;
    private bool isGuiInitialized;
    private RenderFrame[] FrameHandlers { get; }

    /// <summary>
    /// IsHealthy
    /// </summary>
    public override bool IsHealthy => isGuiInitialized;

    /// <summary>
    ///   True if the GUI is being shown
    /// </summary>
    public bool ShowGui { set; get; }

    public ConsoleGuiModule(params RenderFrame[] renderFrames) {
      FrameHandlers = renderFrames;
    }

    public override Task Run() {
      Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

      // Show GUI once all startup tasks are healthy
      Task.Run(async () => {
        while (Kernel.GetAll<AbstractTaskModule>().ToList().Any(x => !x.IsHealthy)) {
          await Task.Delay(1000);
        }
        await Task.Delay(2000);
        ShowGui = true;
      });

      var t = new Timer();
      t.Interval = UpdateInterval;
      t.Elapsed += (o, e) => UpdateGui();
      t.Start();
      isGuiInitialized = true;
      return Task.CompletedTask;
    }

    protected override void ConfigureModules() {
    }

    private void UpdateGui() {
      if (!ShowGui) {
        return;
      }

      if (frame++ % 10 == 0) {
        Console.Clear();
      }

      foreach (var handler in FrameHandlers) {
        handler(helper);
      }
    }

    /// <summary>
    ///   GUI Helper for drawing UIs
    /// </summary>
    public class GuiHelper {
      /// <summary>
      ///   Draws the title
      /// </summary>
      /// <param name="texts"></param>
      public void DrawTitle(params string[] texts) {
        if (texts.Length == 0) {
          return;
        }
        var perTitleWidth = Console.WindowWidth / texts.Length;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.SetCursorPosition(0, 0);

        // Draw each segment + clear whitespace
        var i = 0;
        foreach (var txt in texts) {
          Console.Write(txt);
          if (txt.Length <= perTitleWidth) {
            Console.Write("".PadLeft(perTitleWidth - txt.Length, ' '));
          } else {
            Console.SetCursorPosition(perTitleWidth * (i + 1), 0);
          }
          i++;
        }
      }

      /// <summary>
      ///   Draws a header for ProgressBar[] stats
      /// </summary>
      public void DrawTableHeader(int x, int y, string text) {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.SetCursorPosition(x, y);
        Console.Write(text);
      }

      /// <summary>
      ///   /Draws a progress bar at the given position
      /// </summary>
      public void DrawProgressBar(int x, int y, int maxBarLen, int maxNameLen, string name, double percent,
        string valStr) {
        var barLen = maxBarLen - maxNameLen;
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(x, y);
        Console.Write(name.PadLeft(maxNameLen));

        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(x + maxNameLen + 1, y);
        Console.Write("[");
        Console.ForegroundColor = percent <= 1.0
          ? (percent <= 0.75 ? ConsoleColor.Gray : ConsoleColor.Yellow)
          : ConsoleColor.Red;

        var full = (int) Math.Round(barLen * Math.Min(1.0, percent));
        Console.Write("".PadLeft(full, '='));
        Console.Write("".PadLeft(barLen - full, ' '));

        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("] ");

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write(valStr);
      }

      /// <summary>
      ///   /Draws a progress bar at the given position
      /// </summary>
      public void DrawTableValue(int x, int y, int maxNameLen, string name, string valStr)
      {
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(x, y);
        Console.Write(name.PadLeft(maxNameLen));

        Console.SetCursorPosition(x + maxNameLen + 1, y);
        Console.Write(": ");

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write(valStr);
      }

      /// <summary>
      /// Draws a table of counters
      /// </summary>
      public void DrawCounters<TName,TValue>(int x, int y, string headerName, IEnumerable<KeyValuePair<TName,TValue>> values, Func<TValue, double> percentFunc, int? maxEntries = null, Func<TValue, string> valueFormat = null, int maxBarLen = 32, int maxNameLen = 20) where TValue : IComparable<TValue> {
        DrawTableHeader(x, y++, headerName);
        var max = Math.Min(maxEntries ?? int.MaxValue, Console.WindowHeight - y);
        if (valueFormat == null) {
          valueFormat = (v) => v.ToString();
        }
        foreach (var entry in values.ToArray().OrderByDescending(kv => kv.Value).Take(max)) {
          DrawProgressBar(x, y++, maxBarLen, maxNameLen, entry.Key.ToString(), percentFunc(entry.Value), valueFormat(entry.Value));
        }
      }

      /// <summary>
      /// Draws a table
      /// </summary>
      public void DrawTable<TName, TValue>(int x, int y, string headerName, IEnumerable<KeyValuePair<TName, TValue>> values, int? maxEntries = null, Func<TValue, string> valueFormat = null, int maxNameLen = 15)
      {
        DrawTableHeader(x, y++, headerName);
        var max = Math.Min(maxEntries ?? int.MaxValue, Console.BufferHeight - y);
        if (valueFormat == null)
        {
          valueFormat = (v) => v.ToString();
        }
        foreach (var entry in values.ToArray().Take(max))
        {
          DrawTableValue(x, y++, maxNameLen, entry.Key.ToString(), valueFormat(entry.Value));
        }
      }
    }
  }
}