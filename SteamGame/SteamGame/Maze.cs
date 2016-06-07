using System;
using System.Collections;

namespace SteamGame
{
    public class Maze
    {
        public int xDim, yDim;
        public Cell[,] Cells = null;
        Stack CellStack = new Stack();
        int VisitedCells = 1;
        int TotalCells;
        Cell CurrentCell = null;

        public Maze(int w, int h)
        {
            xDim = w; yDim = h;
            TotalCells = xDim * yDim;
            Initialize();
        }

        private ArrayList GetNeighborsWithWalls(Cell aCell)
        {
            ArrayList Neighbors = new ArrayList();
            for (int countRow = -1; countRow <= 1; countRow++)
                for (int countCol = -1; countCol <= 1; countCol++)
                    if ((aCell.Row + countRow < xDim) && (aCell.Column + countCol < yDim) && (aCell.Row + countRow >= 0) && (aCell.Column + countCol >= 0) && ((countCol == 0) || (countRow == 0)) && (countRow != countCol))
                        if (Cells[aCell.Row + countRow, aCell.Column + countCol].HasAllWalls())
                            Neighbors.Add(Cells[aCell.Row + countRow, aCell.Column + countCol]);
            return Neighbors;
        }

        public void Initialize()
        {
            Cells = new Cell[xDim, yDim];
            TotalCells = xDim * yDim;
            for (int i = 0; i < xDim; i++)
                for (int j = 0; j < yDim; j++)
                {
                    Cells[i, j] = new Cell(this);
                    Cells[i, j].Row = i;
                    Cells[i, j].Column = j;
                }
            CurrentCell = Cells[0, 0];
            VisitedCells = 1;
            CellStack.Clear();
        }

        public void Generate()
        {
            while (VisitedCells < TotalCells)
            {
                ArrayList AdjacentCells = GetNeighborsWithWalls(CurrentCell);
                if (AdjacentCells.Count > 0)
                {
                    int randomCell = Cell.TheRandom.Next(0, AdjacentCells.Count);
                    Cell theCell = ((Cell)AdjacentCells[randomCell]);
                    CurrentCell.KnockDownWall(theCell);
                    CellStack.Push(CurrentCell);
                    CurrentCell = theCell;
                    VisitedCells++;
                }
                else CurrentCell = (Cell)CellStack.Pop();
            }
        }
    }

    public class Cell
    {
        public static int kCellSize = 10;
        public static int kPadding = 5;
        public bool[] Walls = new bool[4] { true, true, true, true };
        public int Row;
        public int Column;
        private static long Seed = DateTime.Now.Ticks - 1;
        static public Random TheRandom = new Random((int)Cell.Seed);
        private Maze Owner;

        public Cell(Maze Owner)
        {
            this.Owner = Owner;
        }

        public bool HasAllWalls()
        {
            for (int i = 0; i < 4; i++) if (!Walls[i]) return false;
            return true;
        }

        public void KnockDownWall(int theWall) { Walls[theWall] = false; }

        public void KnockDownWall(Cell theCell)
        {
            int theWallToKnockDown = FindAdjacentWall(theCell);
            Walls[theWallToKnockDown] = false;
            int oppositeWall = (theWallToKnockDown + 2) % 4;
            theCell.Walls[oppositeWall] = false;
        }


        public int FindAdjacentWall(Cell theCell)
        {
            if (theCell.Row == Row)
            {
                if (theCell.Column < Column) return 0;
                else return 2;
            }
            else
            {
                if (theCell.Row < Row) return 1;
                else return 3;
            }
        }

        public int GetRandomWall()
        {
            int nextWall = TheRandom.Next(0, 3);
            while ((!Walls[nextWall]) || ((Row == 0) && (nextWall == 0)) || ((Row == Owner.xDim - 1) && (nextWall == 2)) || ((Column == 0) && (nextWall == 1)) || ((Column == Owner.yDim - 1) && (nextWall == 3)))
                nextWall = TheRandom.Next(0, 3);
            return nextWall;
        }
    }
}
