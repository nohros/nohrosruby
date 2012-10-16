using System;

namespace Nohros.Ruby
{
  /// <summary>
  /// Contains the values of exception codes defined for Ruby.
  /// </summary>
  public enum ExceptionCode
  {
    /// <summary>
    /// The request could not be understood by the service due
    /// to a malformed syntax.
    /// </summary>
    BadRequest = 400,

    /// <summary>
    /// The service encountered an unexpected condition which prevented
    /// if from fulfilling the request.
    /// </summary>
    ServerError = 500
  }
}
