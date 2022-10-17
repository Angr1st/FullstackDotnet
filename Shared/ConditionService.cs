namespace Lib.Shared
{
    public class ConditionService
    {
        private Action Action { get; init; }

        private Func<bool>[] Condtitions { get; init; }

        public ConditionService(Action action, params Func<bool>[] conditions)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action), "Action is required for ConditionService!");
            }
            Action = action;

            if (conditions is null)
            {
                throw new ArgumentNullException(nameof(conditions));
            }
            if (conditions.Length == 0)
            {
                throw new ArgumentException("You need to supply at least one condition.", nameof(conditions));
            }
            Condtitions = conditions;
        }

        public void EvaluateConditions()
        {
            if (Condtitions.Any(func => func()))
            {
                Action();
            }
        }
    }
}
