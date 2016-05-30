using System;
using System.Collections.Generic;

namespace redis_com_client
{
    public class MyTable
    {
        public List<List<object>> ArrayCollumns {get; set;} //This typo is to make it different
        public List<object> ArrayCollumn { get; set; } //This typo is to make it different 

        public MyTable()
        {
            ArrayCollumns = new List<List<object>>();
            ArrayCollumn = new List<object>();
        }

        public MyTable(object[,] array) : this()
        {
            for (var column = 0; column < array.GetLength(0); column++)
            {
                ArrayCollumns.Add(new List<object>());
            }

            for (var column = 0; column < array.GetLength(0); column++)
            {
                for (var row = 0; row < array.GetLength(1); row++)
                {
                    var value = array[column, row] == DBNull.Value ? null : array[column, row];
                    ArrayCollumns[column].Add(value);
                }
            }
        }

        public MyTable(object[] array) : this()
        {
            for (var column = 0; column < array.GetLength(0); column++)
            {
                var value = array[column] == DBNull.Value ? null : array[column];
                ArrayCollumn.Add(value);
            }
        }

        public object GetArray()
        {
            if (ArrayCollumns.Count > 0)
            {
                var newArrayTwoDim = new object[ArrayCollumns.Count, ArrayCollumns[0].Count];
                for (var i = 0; i < ArrayCollumns.Count; i++)
                {
                    for (var j = 0; j < ArrayCollumns[i].Count; j++)
                    {
                        object value;
                        if (ArrayCollumns[i][j] is long)
                            value = int.Parse(ArrayCollumns[i][j].ToString());
                        else
                            value = ArrayCollumns[i][j];

                        newArrayTwoDim[i,j] = value;
                    }
                }
                return newArrayTwoDim;
            }

            var newArrayOneDim = new object[ArrayCollumn.Count];
            for (var j = 0; j < ArrayCollumn.Count; j++)
            {
                object value;
                if (ArrayCollumn[j] is long)
                    value = int.Parse(ArrayCollumn[j].ToString());
                else
                    value = ArrayCollumn[j];

                newArrayOneDim[j] = value;
            }
            return newArrayOneDim;
        }
        
    }
}