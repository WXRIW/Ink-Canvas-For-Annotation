using System.Collections.Generic;
using System.Windows.Ink;

namespace Ink_Canvas.Helpers
{
    public class TimeMachine
    {
        private readonly List<TimeMachineHistory> _allHistory = new List<TimeMachineHistory>();
        private int _currentIndex = -1;

        private int _currentImageId = -1;

        public delegate void OnUndoStateChange(bool status);

        public event OnUndoStateChange OnUndoStateChanged;

        public delegate void OnRedoStateChange(bool status);

        public event OnRedoStateChange OnRedoStateChanged;

        public void CommitStrokeUserInputHistory(StrokeCollection stroke)
        {
            _allHistory.Add(new TimeMachineHistory(stroke, TimeMachineHistoryType.UserInput, false));
            _currentIndex = _allHistory.Count - 1;
            NotifyUndoRedoState();
        }

        public void CommitStrokeShapeHistory(StrokeCollection strokeToBeReplaced, StrokeCollection generatedStroke)
        {
            _allHistory.Add(new TimeMachineHistory(generatedStroke, TimeMachineHistoryType.ShapeRecognition, false, strokeToBeReplaced));
            _currentIndex = _allHistory.Count - 1;
            NotifyUndoRedoState();
        }

        public void CommitStrokeRotateHistory(StrokeCollection strokeToBeReplaced, StrokeCollection generatedStroke)
        {
            _allHistory.Add(new TimeMachineHistory(generatedStroke, TimeMachineHistoryType.Rotate, false, strokeToBeReplaced));
            _currentIndex = _allHistory.Count - 1;
            NotifyUndoRedoState();
        }

        public void CommitStrokeEraseHistory(StrokeCollection stroke, StrokeCollection sourceStroke = null)
        {
            _allHistory.Add(new TimeMachineHistory(stroke, TimeMachineHistoryType.Clear, true, sourceStroke));
            _currentIndex = _allHistory.Count - 1;
            NotifyUndoRedoState();
        }

        public void CommitStrokeImageHistory(byte[] imageBytes, double x, double y, double width, double height)
        {
            _currentImageId++;
            _allHistory.Add(new TimeMachineHistory(imageBytes, x, y, width, height, TimeMachineHistoryType.ImageInsert, _currentImageId));
            _currentIndex = _allHistory.Count - 1;
            NotifyUndoRedoState();
        }
        public void CommitImageDeleteHistory(int imageId)
        {
            _allHistory.Add(new TimeMachineHistory(TimeMachineHistoryType.ImageRomove ,imageId));
            _currentIndex = _allHistory.Count - 1;
            NotifyUndoRedoState();
        }
        public void CommitImageChangeHistory(double x, double y, double width, double height, int imageId)
        {
            _allHistory.Add(new TimeMachineHistory(x, y, width, height, imageId));
            _currentIndex = _allHistory.Count - 1;
            NotifyUndoRedoState();
        }

        public void ClearStrokeHistory()
        {
            _allHistory.Clear();
            _currentIndex = -1;
            NotifyUndoRedoState();
        }

        public TimeMachineHistory Undo()
        {
            if (_currentIndex >= 0)
            {
                var item = _allHistory[_currentIndex];
                item.StrokeHasBeenCleared = !item.StrokeHasBeenCleared;
                _currentIndex--;
                OnUndoStateChanged?.Invoke(_currentIndex > -1);
                OnRedoStateChanged?.Invoke(_allHistory.Count - _currentIndex - 1 > 0);
                return item;
            }
            return null;
        }

        public TimeMachineHistory Redo()
        {
            if (_currentIndex < _allHistory.Count - 1)
            {
                var item = _allHistory[++_currentIndex];
                item.StrokeHasBeenCleared = !item.StrokeHasBeenCleared;
                NotifyUndoRedoState();
                return item;
            }
            return null;
        }

        public TimeMachineHistory[] ExportTimeMachineHistory()
        {
            return _allHistory.ToArray();
        }

        public bool ImportTimeMachineHistory(TimeMachineHistory[] sourceHistory)
        {
            // 清除所有历史记录
            _allHistory.Clear();
            _currentIndex = -1;
            
            // 遍历导入的历史记录
            foreach (var history in sourceHistory)
            {
                _allHistory.Add(history);
            }
            _currentIndex = _allHistory.Count - 1;

            NotifyUndoRedoState();
            return true;
        }
        private void NotifyUndoRedoState()
        {
            OnUndoStateChanged?.Invoke(_currentIndex > -1);
            OnRedoStateChanged?.Invoke(_allHistory.Count - _currentIndex > 1);
        }
    }

    public class ImageData
    {
        public byte[] ImageBytes { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }

    public class TimeMachineHistory 
    {
        public TimeMachineHistoryType CommitType;
        public bool StrokeHasBeenCleared;
        public StrokeCollection CurrentStroke;
        public StrokeCollection ReplacedStroke;
        public byte[] ImageBytes;
        public double X;
        public double Y;
        public double Width;
        public double Height;
        public int ImageId;
        public string ImageSavedName;

        public TimeMachineHistory(StrokeCollection currentStroke, TimeMachineHistoryType commitType, bool strokeHasBeenCleared)
        { //墨迹
            CommitType = commitType;
            CurrentStroke = currentStroke;
            StrokeHasBeenCleared = strokeHasBeenCleared;
            ReplacedStroke = null;
            ImageBytes = null;
            X = 0;            
            Y = 0;            
            Width = 0;        
            Height = 0;       
        }

        public TimeMachineHistory(StrokeCollection currentStroke, TimeMachineHistoryType commitType, bool strokeHasBeenCleared, StrokeCollection replacedStroke)
        { //墨迹替换
            CommitType = commitType;
            CurrentStroke = currentStroke;
            StrokeHasBeenCleared = strokeHasBeenCleared;
            ReplacedStroke = replacedStroke;
            ImageBytes = null;
            X = 0;            
            Y = 0;            
            Width = 0;        
            Height = 0;       
        }

        public TimeMachineHistory(byte[] imageBytes, double x, double y, double width, double height, TimeMachineHistoryType commitType, int imageId)
        { // 添加图片
            CommitType = commitType;
            CurrentStroke = null;
            StrokeHasBeenCleared = false;
            ReplacedStroke = null;
            ImageBytes = imageBytes;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            ImageId = imageId;
        }
        public TimeMachineHistory(TimeMachineHistoryType commitType, int imageId)
        { // 删除图片
            CommitType = commitType;
            ImageId = imageId;
        }
        public TimeMachineHistory(double x, double y, double width, double height, int imageId)
        { // 变换图片
            CommitType = TimeMachineHistoryType.ImageChange;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            ImageId = imageId;
        }
    }

    public enum TimeMachineHistoryType
    {
        UserInput,
        ShapeRecognition,
        Clear,
        Rotate,
        ImageInsert,
        ImageRomove,
        ImageChange
    }
}