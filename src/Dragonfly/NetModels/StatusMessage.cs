namespace Dragonfly.NetModels
{
	using Newtonsoft.Json;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net.Http;
	using System.Text;

	/// <summary>
	/// Object used for collecting and reporting information about code operations.
	/// </summary>
	public class StatusMessage
	{
		private Type _relatedObjectType;
		private string _relatedObjectTypeName;
		private object _relatedObject;
		private Exception _relatedException;
		private Dictionary<string, string> _relatedExceptionInfo;
        private List<string> _detailedMessages;

        #region Properties

        /// <summary>
        /// Represents whether the operation completed successfully.
        /// </summary>
        public bool Success { get; set; }

		/// <summary>
		/// Name of the Function, method, etc. that generated this StatusMessage 
		/// </summary>
		public string RunningFunctionName { get; set; }

		/// <summary>
		/// Status Message
		/// </summary>
		public string Message { get; set; }

        /// <summary>
        /// More detailed Status Message (multiple strings)
        /// </summary>
        public List<string> DetailedMessages
        {
            set => _detailedMessages = value;

            get
            {
                if (!string.IsNullOrEmpty(this.MessageDetails))
                {
                    var allMsg = _detailedMessages;
					allMsg.Add(this.MessageDetails);
                    return allMsg;
                }
                else
                {
                    return _detailedMessages; 
                }
            } 
        }

        /// <summary>
        /// More detailed Status Message
        /// </summary>
        [Obsolete("Consider using the 'DetailedMessages' property")]
		public string MessageDetails { get; set; }

		/// <summary>
		/// Short representative Code
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// Timestamp for start of message or operations
		/// </summary>
		/// <remarks>If not specified specifically, this will represent the time the StatusMessage object was created.</remarks>
		public DateTime TimestampStart { get; set; }

		/// <summary>
		/// Timestamp for end of operations
		/// </summary>
		/// <remarks>Must be specified specifically. Useful when attempting to time code operations</remarks>
		public DateTime? TimestampEnd { get; set; }

		/// <summary>
		/// Exceptions which occur can be attached here
		/// </summary>
		[JsonIgnore]
		[Obsolete("Use SetRelatedException() and GetRelatedException() methods. This was changed due to Serialization issues")]
		public Exception RelatedException
		{
			set { _relatedException = value; }
			//get { return null; }

		}

		/// <summary>
		/// Exceptions which occur can be attached here
		/// </summary>
		//[JsonIgnore]
		public Exception GetRelatedException()
		{
			return _relatedException;
		}

		/// <summary>
		/// Exceptions which occur can be attached here
		/// </summary>
		//[JsonIgnore]
		public void SetRelatedException(Exception value)
		{
			_relatedException = value;

			if (value != null)
			{
				var info = new Dictionary<string, string>();
				info.Add("Message", value.Message);
				info.Add("Source", value.Source);
				info.Add("StackTrace", value.StackTrace);
				if (value.InnerException != null)
				{ info.Add("InnerException.Message", value.InnerException.Message); }

				_relatedExceptionInfo = info;
			}
		}

		/// <summary>
		/// Serializable Exception Info
		/// </summary>
		public Dictionary<string, string> RelatedExceptionInfo
		{
			get
			{
				return _relatedExceptionInfo;
			}
		}

		/// <summary>
		/// Status Messages can be nested
		/// </summary>
		public List<StatusMessage> InnerStatuses { get; set; }

		/// <summary>
		/// Name of the Object that this status message refers to
		/// </summary>
		public string ObjectName { get; set; }

		/// <summary>
		/// Int Id of the Object that this status message refers to
		/// </summary>
		public int ObjectId { get; set; }

		/// <summary>
		/// Guid of the Object that this status message refers to
		/// </summary>
		public string ObjectGuid { get; set; }

		public Type RelatedObjectType()
		{
			return _relatedObjectType;
		}
		
		/// <summary>
		/// Type name of the Related Object
		/// </summary>
		public string RelatedObjectTypeName
		{
			get => _relatedObjectTypeName;
		}

		/// <summary>
		/// Object which can be appended for additional information
		/// </summary>
		public object RelatedObject
		{
			get => _relatedObject;
			set
			{
				_relatedObject = value;

				if (value != null)
				{
					_relatedObjectType = value.GetType();
					_relatedObjectTypeName = _relatedObjectType.ToString();
				}
			}
		}

		/// <summary>
		/// Type of the Related Object
		/// </summary>
		//[JsonIgnore]
	

		#endregion


		#region Constructors

		public StatusMessage()
		{
			SetDefaults();
		}

		public StatusMessage(DateTime StartTimestamp)
		{
			SetDefaults();
			this.TimestampStart = StartTimestamp;
		}

		public StatusMessage(bool WasSuccessful, DateTime? StartTimestamp = null)
		{
			SetDefaults();
			this.Success = WasSuccessful;

			if (StartTimestamp != null)
			{
				this.TimestampStart = (DateTime)StartTimestamp;
			}
			else
			{
				this.TimestampStart = DateTime.Now;
			}

		}

		public StatusMessage(bool WasSuccessful, string Msg, DateTime? StartTimestamp = null)
		{
			SetDefaults();
			this.Success = WasSuccessful;
			this.Message = Msg;

			if (StartTimestamp != null)
			{
				this.TimestampStart = (DateTime)StartTimestamp;
			}
			else
			{
				this.TimestampStart = DateTime.Now;
			}
		}

		private void SetDefaults()
		{
			this.InnerStatuses = new List<StatusMessage>();
			this.DetailedMessages = new List<string>();
			this.TimestampStart = DateTime.Now;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Duration between TimestampStart and TimestampEnd
		/// </summary>
		/// <returns></returns>
		public TimeSpan? TimeDuration()
		{
			if (TimestampEnd != null)
			{
				var duration = (DateTime)TimestampEnd - TimestampStart;
				return duration;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Returns TRUE if there is content in the 'Message' property
		/// </summary>
		public bool HasMessage()
		{
			if (this.Message != string.Empty & this.Message != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Checks main and all nested StatusMessages for exceptions. 
		/// </summary>
		/// <returns>Returns TRUE if any Exceptions found.</returns>
		public bool HasAnyExceptions()
		{
			if (this._relatedException != null)
			{
				return true;
			}

			if (this.InnerStatuses.Any())
			{
				return this.InnerStatuses.Select(n => n._relatedException).Any(n => n != null);
			}

			return false;
		}


		/// <summary>
		/// Checks main and all nested StatusMessages for Success=false. 
		/// </summary>
		/// <returns>Returns TRUE if any found.</returns>
		public bool HasAnyFailures()
		{
			if (!this.Success)
			{
				return true;
			}

			if (this.InnerStatuses.Any())
			{
				return this.InnerStatuses.Any(n => n.Success != true);
			}

			return false;
		}

		/// <summary>
		/// Converts StatusMessage into a HttpResponseMessage to return via a WebApi call, for instance.
		/// </summary>
		/// <returns></returns>
		public HttpResponseMessage ToHttpResponse()
		{
			string json = JsonConvert.SerializeObject(this);

			return new HttpResponseMessage()
			{
				Content = new StringContent(
					json,
					Encoding.UTF8,
					"application/json"
				)
			};
		}

		/// <summary>
		/// Converts StatusMessage into a string appropriate for logging in a text file
		/// </summary>
		/// <param name="IndentLevel">Prepends dashes to indent the text for inner statuses (3 x IndentInterval)</param>
		/// <returns></returns>
		public string ToStringForLog(int IndentLevel = 0)
		{
			var sb = new StringBuilder();
			var indent = string.Concat(Enumerable.Repeat("---", IndentLevel)) + " ";

			sb.AppendLine(string.Format("{0}Success: {1}", indent, this.Success));

			if (this.HasMessage())
			{
				sb.AppendLine(string.Format("{0}Message: {1}", indent, this.Message));
				foreach (var message in DetailedMessages)
				{
					sb.AppendLine(message);
				}
			}

			if (this.InnerStatuses.Any())
			{
				sb.AppendLine(string.Format("{0}Inner Statuses:", indent));
				foreach (var message in this.InnerStatuses)
				{
					sb.AppendLine(message.ToStringForLog(IndentLevel + 1));
				}
			}

			return sb.ToString();
		}



		#endregion


	}


}
