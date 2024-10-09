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
        private List<Color> shapeColors = new List<Color>()
        {
            Color.Aqua, Color.Aquamarine, Color.Blue, Color.Chartreuse, Color.Crimson, Color.DarkOliveGreen, Color.HotPink,
            Color.Orange, Color.Sienna, Color.SkyBlue, Color.Tan, Color.Yellow, Color.Red, Color.LavenderBlush, Color.Gold,
            Color.DarkKhaki, Color.DarkCyan
        };
        private Random random = new Random();
        private List<Grid> generateShapes()
        {
            List<Grid> shapes = new List<Grid>();
            int remainingCells = gameBoardSize * gameBoardSize;
            List<Color> availableColors = new List<Color>(shapeColors);
            bool[,] occupiedGrid = new bool[gameBoardSize, gameBoardSize];

            while (remainingCells > 0)
            {
                int startX = -1, startY = -1;
                for (int i = 0; i < gameBoardSize; i++)
                {
                    for (int j = 0; j < gameBoardSize; j++)
                    {
                        if (!occupiedGrid[i, j])
                        {
                            startX = i;
                            startY = j;
                            break;
                        }
                    }
                    if (startX != -1) break;  // znaleziono wolne miejsce
                }

                if (startX == -1 || startY == -1)
                {
                    break; // brak wolnych miejsc
                }

                int maxWidth = Math.Min(3, gameBoardSize - startY);  // max szerokosc - nie przekracza dostepnych kolumn
                int maxHeight = Math.Min(3, gameBoardSize - startX);  // Mmax wysokosc - nie przekracza dostepnych wierszy
                int width = random.Next(1, maxWidth + 1);
                int height = random.Next(1, maxHeight + 1);

                bool canPlace = true;
                for (int i = startX; i < startX + height && canPlace; i++)
                {
                    for (int j = startY; j < startY + width; j++)
                    {
                        if (occupiedGrid[i, j])
                        {
                            canPlace = false;
                            break;
                        }
                    }
                }

                if (canPlace)
                {
                    remainingCells -= width * height; // odejmij od potrzebnych klockow pole ksztaltu

                    Color shapeColor = availableColors[random.Next(availableColors.Count)];
                    availableColors.Remove(shapeColor);

                    Grid shape = new Grid
                    {
                        ColumnSpacing = 3,
                        RowSpacing = 3
                    };

                    for (int i = 0; i < height; i++)
                    {
                        shape.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) });
                    }
                    for (int j = 0; j < width; j++)
                    {
                        shape.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });
                    }

                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            BoxView boxView = new BoxView
                            {
                                Color = shapeColor,
                                CornerRadius = new CornerRadius(3)
                            };

                            Grid.SetRow(boxView, i);
                            Grid.SetColumn(boxView, j);
                            shape.Children.Add(boxView);
                        }
                    }

                    // aktualizacja zajetych miejsc na planszy
                    for (int i = startX; i < startX + height; i++)
                    {
                        for (int j = startY; j < startY + width; j++)
                        {
                            occupiedGrid[i, j] = true;
                        }
                    }

                    var dragGestureRecognizer = new DragGestureRecognizer
                    {
                        CanDrag = true
                    };
                    dragGestureRecognizer.DragStarting += DragGestureRecognizer_DragStarting;
                    shape.GestureRecognizers.Add(dragGestureRecognizer);

                    shapes.Add(shape);
                }
            }

            return shapes;
        }

        private void addShapesToPanel()
        {
            var shapes = generateShapes();
            shapesPanel.Children.Clear();

            foreach (var shape in shapes)
            {
                shapesPanel.Children.Add(shape);
            }
        }

        private void assignDropGestures()
        {
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

        private void resetOccupied()
        {
            for (int i = 0; i < gameBoardSize; i++)
            {
                for (int j = 0; j < gameBoardSize; j++)
                {
                    isOccupied[i, j] = false;
                }
            }
        }
        public MainPage()
        {
            InitializeComponent();
            generateFields();
            isOccupied = new bool[gameBoardSize, gameBoardSize];
            addShapesToPanel();
            StartingShapes = shapesPanel.Children.OfType<Grid>().ToList();
            assignDropGestures();
        }

        public void generateFields()
        {
            for (int i = 0; i < gameBoardSize; i++)
            {
                for (int j = 0; j < gameBoardSize; j++)
                {
                    BoxView boxView = new BoxView()
                    {
                        Color = Color.FromHex("#454545"),
                        CornerRadius = new CornerRadius(3)
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
                MyParticleCanvas.IsVisible = false;
                MyParticleCanvas.IsVisible = true;
                Console.WriteLine(MyParticleCanvas.IsActive + "   " + MyParticleCanvas.IsRunning + "   " + MyParticleCanvas.IsVisible);
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
                                    Color = ((BoxView)child).Color,
                                    CornerRadius = new CornerRadius(3)
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

        private void Reset_Clicked(object sender, EventArgs e)
        {
            GameBoardGrid.Children.Clear();
            shapesPanel.Children.Clear();

            MyParticleCanvas.IsVisible = false;

            generateFields();
            resetOccupied();
            assignDropGestures();

            foreach (Grid item in StartingShapes)
            {
                shapesPanel.Children.Add(item);
            }
        }

        private void Randomize_Clicked(object sender, EventArgs e)
        {
            GameBoardGrid.Children.Clear();
            shapesPanel.Children.Clear();

            MyParticleCanvas.IsVisible = false;

            generateFields();
            resetOccupied();
            assignDropGestures();
            addShapesToPanel();
        }
    }
}