
public enum UserRole
{
    FactoryManager,
    QualityController,
    ProductionSupervisor,
    Technician,
    PackagingStaff,
    MaterialStaff,
    SalesRepresentative
}

public enum Unit
{
    Kilogram,
    Gram,
    Liter,
    Milliliter,
    Piece
}

public enum Shift
{
    Morning,
    Evening,
    Night
}

public enum ProductionStatus
{
    Scheduled,
    InProgress,
    Completed,
    Canceled
}

public enum QualityStatus
{
    Approved,
    Rejected,
    Pending
}

public enum NotificationType
{
    Alert,
    Reminder,
    Information
}
