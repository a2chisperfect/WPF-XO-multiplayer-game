using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message
{
  
    public class Field
    {
        Side[,] field;
        public Field()
        {
            field = new Side[3, 3];
        }
        public void ClearField()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    field[i, j] = Side.empty;
                }
            }
        }
        public Side GetCell(int x, int y)
        {
            return field[x, y];
        }
        public void SetCell(int x, int y, Side s)
        {
            field[x, y] = s;
        }
        public int FreeCells()
        {
            int tmp = 0;
            foreach (var cell in field)
            {
                if (cell == Side.empty) tmp++;
            }
            return tmp;
        }
    }
}
