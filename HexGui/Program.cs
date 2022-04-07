using System;
using Terminal.Gui;

namespace HexGui
{
    internal static class Program
    {
        public static void Main()
        {
            Application.Init();
            Console.Title = "Hex Something";
            Application.Resized += eventArgs => { FileParser.Refresh((eventArgs.Cols - 15) / 5); };

            var menu = new MenuBar(
                new[]
                {
                    new MenuBarItem("_File",
                        new[]
                        {
                            new MenuItem("_Exit", "", () => { Application.RequestStop(); }),
                            new MenuItem("_Load New", "Loads a new file.", LoadNewFilePrompt),
                            new MenuItem("_Reload", "Reloads the current file.", FileParser.ReloadFile)
                        })
                }
            );

            var win = new Window("Hex Util")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            var gotoAddressInput = new TextField("")
            {
                X = 1,
                Y = 1,
                Width = 9
            };

            var gotoAddressButton = new Button("Go to Address")
            {
                X = gotoAddressInput.Frame.Right + 1,
                Y = gotoAddressInput.Frame.Top
            };


            var searchAobInput = new TextField("")
            {
                X = gotoAddressButton.Frame.Right + 6,
                Y = 1,
                Width = 60
            };

            var searchAobButton = new Button("Find AOB")
            {
                X = searchAobInput.Frame.Right + 1,
                Y = searchAobInput.Frame.Top
            };


            var tbl = new TableView
            {
                X = 1,
                Y = 3,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1
            };


            tbl.AutoSize = true;
            tbl.Table = FileParser.Table;
            tbl.MultiSelect = true;
            tbl.Style.AlwaysShowHeaders = true;


            searchAobButton.Clicked += () => { LocateAob(searchAobInput.Text.ToString(), tbl); };

            gotoAddressButton.Clicked += () =>
            {
                var line = Convert.ToInt32(gotoAddressInput.Text.ToString(), 16) / FileParser.BytesPerLine;
                NavigateTable(tbl, line);
            };


            win.Add(gotoAddressButton, gotoAddressInput, searchAobInput, searchAobButton, tbl);

            Application.Top.Add(menu, win);
            Application.Run();
        }

        private static void NavigateTable(TableView tbl, int line)
        {
            tbl.SetFocus();
            tbl.SetSelection(1, line, false);
            tbl.EnsureValidSelection();
            tbl.EnsureSelectedCellIsVisible();
        }

        private static void LocateAob(string query, TableView tbl)
        {
            var scanQuery = new ByteScanner.ScanQuery(query);
            var res = ByteScanner.Scan(FileParser.FileBytes, scanQuery);

            var n = MessageBox.Query($"Results for [{scanQuery.InputStr}]",
                res > 0 ? $"AOB found at: {res:X8}." : "AOB not found.",
                "Take me there", "Return");
            if (n == 0) NavigateTable(tbl, res / FileParser.BytesPerLine);
        }

        private static void LoadNewFilePrompt()
        {
            while (true)
            {
                var selector = new OpenDialog("File to open", "Select a file.")
                {
                    CanChooseDirectories = false,
                    CanChooseFiles = true
                };

                Application.Run(selector);

                if (selector.Canceled)
                    break;

                if (selector.FilePath == null) continue;

                var n = MessageBox.Query(90, 10,
                    "Load this File?", selector.FilePath, "Load", "Return");

                if (n != 0) continue;

                FileParser.ReadNewFile(selector.FilePath.ToString());
                break;
            }
        }
    }
}