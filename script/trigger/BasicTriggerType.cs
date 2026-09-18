public enum BasicTriggerType {
    /// <summary>
    /// 已经确定为真，且不会再变
    /// </summary>
    alwaysTrue,
    /// <summary>
    /// 目前是真，但可能会变
    /// </summary>
    currentTrue,
    /// <summary>
    /// 目前是假，但可能会变
    /// </summary>
    currentFalse,
    /// <summary>
    /// 已经确定为假，且不会再变
    /// </summary>
    alwaysFalse
}
