namespace ToDoList.Common;

public abstract class BusinessException : Exception
{
    protected BusinessException(string message, Exception? innerException = null) : base(message, innerException) { }
}

public abstract class AccessForbiddenException : Exception
{
    protected AccessForbiddenException(string message, Exception? innerException = null) : base(message, innerException) { }
}

public class InvalidParameterException : BusinessException
{
    public InvalidParameterException(string message, Exception? innerException = null) : base(message, innerException) { }
}

public class EntityNotFoundException : BusinessException
{
    public EntityNotFoundException(string message, Exception? innerException = null) : base(message, innerException) { }
}

public class ProjectHasToDoAssociatedException : BusinessException
{
    public ProjectHasToDoAssociatedException(string message, Exception? innerException = null) : base(message, innerException) { }
}
