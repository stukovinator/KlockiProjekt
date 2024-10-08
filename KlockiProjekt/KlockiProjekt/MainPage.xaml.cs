using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;

namespace KlockiProjekt
{
    public static class VisualElementExtensions
    {
        public static T FindAncestor<T>(this VisualElement element) where T : VisualElement
        {
            while (element != null && !(element is T))
            {
                element = element.Parent as VisualElement;
            }
            return element as T;
        }
    }

    public partial class MainPage : ContentPage
    {
        private int gameBoardSize = 6;
        private List<Grid> StartingShapes;
        private bool[,] isOccupied;
        public MainPage()
        {
            InitializeComponent();
            generateFields();
            isOccupied = new bool[gameBoardSize, gameBoardSize];
            StartingShapes = shapesPanel.Children.OfType<Grid>().ToList();

            foreach (var child in GameBoardGrid.Children)
            {
                if (child is BoxView)
                {
                    var dropGestureRecognizer = new DropGestureRecognizer
                    {
                        AllowDrop = true
                    };
                    dropGestureRecognizer.Drop += DropGestureRecognizer_Drop;

                    child.GestureRecognizers.Add(dropGestureRecognizer);
                }
            }
        }

        public void generateFields()
        {
            for (int i = 0; i < gameBoardSize; i++)
            {
                for (int j = 0; j < gameBoardSize; j++)
                {
                    BoxView boxView = new BoxView()
                    {
                        Color = Color.FromHex("#454545")
                    };

                    Console.WriteLine($"Adding box at ({i}, {j})");

                    Grid.SetColumn(boxView, j);
                    Grid.SetRow(boxView, i);

                    GameBoardGrid.Children.Add(boxView);
                }
            }
        }

        private void DragGestureRecognizer_DragStarting(object sender, DragStartingEventArgs e)
        {
            var shape = (sender as Element).Parent as Grid;
            e.Data.Properties.Add("Shape", shape);
        }

        private void checkEnd()
        {
            if (shapesPanel.Children.Count == 0)
            {
                Console.WriteLine("WIN");
                MyParticleCanvas.IsVisible = true;
                MyParticleCanvas.IsRunning = true;
                Console.WriteLine(MyParticleCanvas.IsActive +"   " +  MyParticleCanvas.IsRunning + "   " + MyParticleCanvas.IsVisible);
            }
        }

        private bool IsOccupied(int column, int row)
        {
            if (column >= gameBoardSize || row >= gameBoardSize || column < 0 || row < 0)
            {
                return true;
            }

            return isOccupied[column, row];
        }

        private void DropGestureRecognizer_Drop(object sender, DropEventArgs e)
        {
            Console.WriteLine("DROP");

            if (e.Data.Properties.ContainsKey("Shape"))
            {
                var droppedShape = e.Data.Properties["Shape"] as Grid;
                var dropRecognizer = sender as DropGestureRecognizer;
                var dropTarget = (dropRecognizer?.Parent as View).FindAncestor<BoxView>();

                if (dropTarget != null)
                {
                    int dropColumn = Grid.GetColumn(dropTarget);
                    int dropRow = Grid.GetRow(dropTarget);
                    bool canPlace = true;

                    foreach (var child in droppedShape.Children)
                    {
                        if (child is BoxView)
                        {
                            int shapeColumn = Grid.GetColumn(child);
                            int shapeRow = Grid.GetRow(child);
                            int targetColumn = dropColumn + shapeColumn;
                            int targetRow = dropRow + shapeRow;

                            if (IsOccupied(targetColumn, targetRow))
                            {
                                canPlace = false;
                                dropTarget.Color = Color.FromHex("#454545");
                                Console.WriteLine($"Pole ({targetColumn}, {targetRow}) jest już zajęte.");
                                break;
                            }

                            if (targetColumn >= gameBoardSize || targetRow >= gameBoardSize)
                            {
                                canPlace = false;
                                dropTarget.Color = Color.FromHex("#454545");
                                Console.WriteLine("Kształt nie może być postawiony poza planszą.");
                                break;
                            }
                        }
                    }

                    if (canPlace)
                    {
                        foreach (var child in droppedShape.Children)
                        {
                            if (child is BoxView)
                            {
                                int shapeColumn = Grid.GetColumn(child);
                                int shapeRow = Grid.GetRow(child);
                                int targetColumn = dropColumn + shapeColumn;
                                int targetRow = dropRow + shapeRow;

                                var newBoxView = new BoxView()
                                {
                                    Color = ((BoxView)child).Color
                                };

                                Grid.SetColumn(newBoxView, targetColumn);
                                Grid.SetRow(newBoxView, targetRow);
                                GameBoardGrid.Children.Add(newBoxView);
                                isOccupied[targetColumn, targetRow] = true;
                            }
                        }

                        (droppedShape.Parent as StackLayout)?.Children.Remove(droppedShape);
                        checkEnd();
                    }
                    else
                    {

                        Console.WriteLine("Kształt nie może zostać postawiony - miejsce zajęte lub poza planszą");
                    }
                }
                else
                {
                    Console.WriteLine("Wybrane miejsce nie jest boxview");
                }
            }
            else
            {
                Console.WriteLine("Nie znaleziono kształtu");
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            GameBoardGrid.Children.Clear();
            shapesPanel.Children.Clear();
            MyParticleCanvas.IsVisible = false;
            MyParticleCanvas.IsRunning = false;
            generateFields();
            foreach (Grid item in StartingShapes)
            {
                shapesPanel.Children.Add(item);
            }

            foreach (Grid item in shapesPanel.Children)
            {
                var dragGestureRecognizer = new DragGestureRecognizer
                {
                    CanDrag = true,
                };
                dragGestureRecognizer.DragStarting += DragGestureRecognizer_DragStarting;

                item.GestureRecognizers.Add(dragGestureRecognizer);
            }

            foreach (var child in GameBoardGrid.Children)
            {
                if (child is BoxView)
                {
                    var dropGestureRecognizer = new DropGestureRecognizer
                    {
                        AllowDrop = true
                    };
                    dropGestureRecognizer.Drop += DropGestureRecognizer_Drop;

                    child.GestureRecognizers.Add(dropGestureRecognizer);
                }
            }
        }
    }
}