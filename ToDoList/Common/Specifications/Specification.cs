using System.Linq.Expressions;

namespace ToDoList.Common.Specifications
{
    public abstract class Specification<T> where T : class
    {
        public static readonly Specification<T> identification = new SpecificationIdentification<T>();
        public abstract Expression<Func<T, bool>> Rule();
        public bool IsSatisfiedBy(T entity)
        {
            var predicate = Rule().Compile();
            return predicate(entity);
        }
        public Specification<T> And(Specification<T> specification)
        {
            if (this == identification) return specification;
            if (specification == identification) return this;
            return new SpecificationAnd<T>(this, specification);
        }

        public Specification<T> Or(Specification<T> specification)
        {
            if (this == identification || specification == identification) return identification;
            return new SpecificationOr<T>(this, specification);
        }

        public static Specification<T> operator +(Specification<T> x, Specification<T> y) => (x ?? identification).And(y ?? identification);
    }

    internal sealed class SpecificationIdentification<T> : Specification<T> where T : class
    {
        public override Expression<Func<T, bool>> Rule()
        {
            return x => true;
        }
    }

    internal sealed class SpecificationAnd<T> : Specification<T> where T : class
    {
        private readonly Specification<T> left;
        private readonly Specification<T> right;

        public SpecificationAnd(Specification<T> left, Specification<T> right)
        {
            this.left = left;
            this.right = right;
        }

        public override Expression<Func<T, bool>> Rule()
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var leftExpression = left.Rule();
            var rightExpression = right.Rule();
            var body = Expression.AndAlso(
                Expression.Invoke(leftExpression, parameter),
                Expression.Invoke(rightExpression, parameter));
            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
            return lambda;
        }
    }

    internal sealed class SpecificationOr<T> : Specification<T> where T : class
    {
        private readonly Specification<T> left;
        private readonly Specification<T> right;

        public SpecificationOr(Specification<T> left, Specification<T> right)
        {
            this.left = left;
            this.right = right;
        }
        public override Expression<Func<T, bool>> Rule()
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var leftExpression = left.Rule();
            var rightExpression = right.Rule();
            var body = Expression.OrElse(
                Expression.Invoke(leftExpression, parameter),
                Expression.Invoke(rightExpression, parameter));
            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
            return lambda;
        }
    }

    internal sealed class SpecificationNot<T> : Specification<T> where T : class
    {
        private readonly Specification<T> specification;

        public SpecificationNot(Specification<T> specification)
        {
            this.specification = specification;
        }
        public override Expression<Func<T, bool>> Rule()
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var expression = specification.Rule();
            var body = Expression.Not(Expression.Invoke(expression, parameter));
            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
            return lambda;
        }
    }
    public static class SpecificationExtensions
    {
        public static Specification<T> Not<T>(Specification<T> specification) where T : class
        {
            specification.ThrowIfNull();
            return new SpecificationNot<T>(specification);
        }
    }
}
