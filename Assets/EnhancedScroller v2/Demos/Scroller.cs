using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EnhancedUI.EnhancedScroller
{
    /// <summary>
    /// This example shows how to simulate a grid with a fixed number of cells per row
    /// The data is stored as normal, but the differences in this example are:
    /// 
    /// 1) The scroller is told the data count is the number of data elements divided by the number of cells per row
    /// 2) The cell view is passed a reference to the data set with the offset index of the first cell in the row
    public class Scroller<T> : IEnhancedScrollerDelegate
    {
        /// <summary>
        /// Internal representation of our data. Note that the scroller will never see
        /// this, so it separates the data from the layout using MVC principles.
        /// </summary>
        private SmallList<T> _data;
        private float cellSize;
        private int subCellsNumber;

        /// <summary>
        /// This is our scroller we will be a delegate for
        /// </summary>
        private EnhancedScroller scroller;

        /// <summary>
        /// This will be the prefab of each cell in our scroller. The cell view will
        /// hold references to each row sub cell
        /// </summary>
        private EnhancedScrollerCellView cellViewPrefab;

        /// <summary>
        /// Be sure to set up your references to the scroller after the Awake function. The 
        /// scroller does some internal configuration in its own Awake function. If you need to
        /// do this in the Awake function, you can set up the script order through the Unity editor.
        /// In this case, be sure to set the EnhancedScroller's script before your delegate.
        /// 
        /// In this example, we are calling our initializations in the delegate's Start function,
        /// but it could have been done later, perhaps in the Update function.
        /// </summary>
        public Scroller(EnhancedScroller scroller, EnhancedScrollerCellView cellViewPrefab, float cellSize, int subCellsNumber)
        {
            if (scroller == null || cellViewPrefab == null || cellSize < 1 || subCellsNumber < 1)
                throw new System.Exception("Scroller: params invalid");

            this.scroller = scroller;
            this.cellViewPrefab = cellViewPrefab;
            this.cellSize = cellSize;
            this.subCellsNumber = subCellsNumber;

            scroller.Delegate = this;
        }

        /// <summary>
        /// Populates the data with a lot of records
        /// </summary>
        public void Set(List<T> list)
        {
            //if (list == null || list.Count == 0)
            //    throw ExceptionMan.GetException("list empty", "scroller type: " + typeof(T));

            //if (_data != null)
            //    _data.Clear();
            //else
                _data = new SmallList<T>();
            
            if (list != null)
            {
                foreach (var i in list)
                    _data.Add(i);
            }

            //if (list != null)
                //Debug.LogError(list.Count);

            scroller.ReloadData();
        }

        public void ClearActive()
        {
            scroller.ClearActive();

            if (_data != null)
                _data.Clear();
        }

        #region EnhancedScroller Handlers

        /// <summary>
        /// This tells the scroller the number of cells that should have room allocated.
        /// For this example, the count is the number of data elements divided by the number of cells per row (rounded up using Mathf.CeilToInt)
        /// </summary>
        /// <param name="scroller">The scroller that is requesting the data size</param>
        /// <returns>The number of cells</returns>
        public int GetNumberOfCells()
        {
            if (_data != null)
                return Mathf.CeilToInt( _data.Count * 1f / subCellsNumber);
            else
                return 0;
        }

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return GetNumberOfCells();
        }

        /// <summary>
        /// This tells the scroller what the size of a given cell will be. Cells can be any size and do not have
        /// to be uniform. For vertical scrollers the cell size will be the height. For horizontal scrollers the
        /// cell size will be the width.
        /// </summary>
        /// <param name="scroller">The scroller requesting the cell size</param>
        /// <param name="dataIndex">The index of the data that the scroller is requesting</param>
        /// <returns>The size of the cell</returns>
        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return cellSize;
        }

        /// <summary>
        /// Gets the cell to be displayed. You can have numerous cell types, allowing variety in your list.
        /// Some examples of this would be headers, footers, and other grouping cells.
        /// </summary>
        /// <param name="scroller">The scroller requesting the cell</param>
        /// <param name="dataIndex">The index of the data that the scroller is requesting</param>
        /// <param name="cellIndex">The index of the list. This will likely be different from the dataIndex if the scroller is looping</param>
        /// <returns>The cell for the scroller to use</returns>
        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            // first, we get a cell from the scroller by passing a prefab.
            // if the scroller finds one it can recycle it will do so, otherwise
            // it will create a new cell.
            EnhancedScrollerCellView cellView = scroller.GetCellView(cellViewPrefab) as EnhancedScrollerCellView;

            // pass in a reference to our data set with the offset for this cell
            cellView.SetData(ref _data.data, dataIndex);

            // return the cell to the scroller
            return cellView;
        }

        #endregion
    }
}
