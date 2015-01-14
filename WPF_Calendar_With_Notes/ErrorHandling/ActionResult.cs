using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Calendar_With_Notes
{
    public class ActionResult
    {
        private static readonly Lazy<ActionResult> _success = new Lazy<ActionResult>(() => new ActionResult());

        private bool _isSuccess;
        public bool IsSuccess
        {
            get
            {
                return _isSuccess;
            }
        }

        private ActionResult(bool isSuccess = true, string errorMsg = "") { this._isSuccess = isSuccess; this._errorMsg = errorMsg; }

        private string _errorMsg;
        public string Error { get { return _errorMsg; } }

        public static ActionResult CreateSuccessResult()
        {
            return _success.Value;
        }

        public static ActionResult CreateFailResult(string errorMsg)
        {
            return new ActionResult(false, errorMsg);
        }
    }
}
