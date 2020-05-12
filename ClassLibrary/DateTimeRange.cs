using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

namespace ClassLibrary
{
    //
    public struct DateTimeRange : IEquatable<DateTimeRange>, IEqualityComparer<DateTimeRange>, IFormattable
    {
        public DateTime Begin { get; private set; }
        public DateTime End { get; private set; }
        public DateTimeRange(DateTime begin, DateTime end)
        {
            Begin = begin;
            End = end;
        }
        public DateTimeRange(DateTime begin, TimeSpan interval)
        {
            Begin = begin;
            End = Begin.Add(interval);
        }
        public DateTimeRange(DateTime begin, DateTime? end)
        {
            var _end = end ?? DateTime.MaxValue;
            Begin = begin;
            End = _end;
        }
        public static DateTimeRange operator +(DateTimeRange a, TimeSpan b) =>
             new DateTimeRange(a.Begin, a.End.Add(b));

        public bool Equals(DateTimeRange other)
        {
            return Begin == other.Begin && End == other.End;
        }

        public bool Equals(DateTimeRange x, DateTimeRange y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(DateTimeRange obj)
        {
            return HashCode.Combine(obj.Begin, obj.End);
        }
        public static bool operator ==(DateTimeRange a, DateTimeRange b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(DateTimeRange a, DateTimeRange b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj is DateTimeRange x)
            {
                return Equals(x);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        public static IEnumerable<DateTimeRange> Create(IDictionary<DateTime, bool> pulse)
        {
            if (pulse == null)
            {
                throw new ArgumentNullException(nameof(pulse));
            }
            var keys = new List<DateTime>(pulse.Keys);
            keys.Sort();
            if (keys.Count < 1)
            {
                yield break;
            }
            int j;
            for (j = 0; j < keys.Count; j++)
            {
                if (pulse[keys[j]])
                {
                    break;
                }
            }
            if (!pulse[keys[j]])
            {
                yield break;
            }
            DateTime? begin = keys[j];

            for (var i = j + 1; i < keys.Count; i++)
            {
                if (!pulse[keys[i]] && begin.HasValue)
                {
                    yield return new DateTimeRange(begin.Value, keys[i]);
                    begin = null;
                }
                else
                {
                    if (pulse[keys[i]] &&! begin.HasValue)
                    {
                        begin = keys[i];
                    }
                }

            }
        }
        public static IEnumerable<DateTimeRange> Create<T>(IDictionary<DateTime, T> values, T min) where T : IComparable<T>
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            //на случай, если T class
            if (min == null)
            {
                throw new ArgumentNullException(nameof(min));
            }
            var keys = new List<DateTime>(values.Keys);
            keys.Sort();
            if (keys.Count < 1)
            {
                yield break;
            }
            if (keys.Count == 1)
            {
                yield return new DateTimeRange(keys[0], DateTime.MaxValue);
            }
            DateTime? begin = null;
            for (var i = 0; i < keys.Count; i++)
            {
                if (!begin.HasValue && values[keys[i]].CompareTo(min) >= 0)
                {
                    begin = keys[i];
                }
                else
                {
                    if (begin.HasValue && values[begin.Value].CompareTo(values[keys[i]]) > 0)
                    {
                        yield return new DateTimeRange(begin.Value, keys[i]);
                        begin = null;
                    }
                }
            }
            if (begin.HasValue)
            {
                yield return new DateTimeRange(begin.Value, DateTime.MaxValue);

            }
        }
     

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return $"{Begin.ToString(format, formatProvider)} - {End.ToString(format, formatProvider)}";
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class DateTimeRangeExtensions
    {
        public static bool Compare(this IEnumerable<DateTimeRange> a, IEnumerable<DateTimeRange> b)
        {
            if (a == null)
            {
                throw new ArgumentNullException(nameof(a));
            }
            if (b == null)
            {
                throw new ArgumentNullException(nameof(b));
            }
            var ea = a.GetEnumerator();
            var eb = b.GetEnumerator();
            while (true)
            {
                var ba = ea.MoveNext();
                var bb = eb.MoveNext();
                if (ba != bb)//длины не равны
                {
                    return false;
                }
                if (!ba)//достаточно одного
                {
                    return true;
                }
                if (!ea.Current.Equals(eb.Current))
                {
                    return false;
                }
            }
        }
        public static IEnumerable<DateTimeRange> UnionAll(this IEnumerable<DateTimeRange> a, IEnumerable<DateTimeRange> b)
        {
            if (a == null)
            {
                throw new ArgumentNullException(nameof(a));
            }
            if (b == null)
            {
                throw new ArgumentNullException(nameof(b));
            }
            var en = a.GetEnumerator();
            while (en.MoveNext())
            {
                yield return en.Current;
            }
            en = b.GetEnumerator();
            while (en.MoveNext())
            {
                yield return en.Current;
            }
        }
        /// <summary>
        /// как понимаю, для всех одинаковых Begin нужно взыть максимальную End
        /// </summary>
        /// <param name="ranges"></param>
        /// <returns></returns>
        public static IEnumerable<DateTimeRange> Merge(this IEnumerable<DateTimeRange> ranges)
        {
            var list = ranges.Select(x => (x.Begin, x.End))
                .OrderBy(x => x.Begin)
                .ThenBy(x => x.End)
                .ToList();
            if (list.Count < 1)
            {
                yield break;
            }
            if (list.Count == 1)
            {
                yield return new DateTimeRange(list[0].Begin, list[0].End);
            }
            var begin = list[0];
            for (var i = 1; i < list.Count; i++)
            {
                var l = list[i];
                if (begin.Begin.Hour < l.Begin.Hour)
                {
                    yield return new DateTimeRange(begin.Begin, list[i - 1].End);
                    begin = l;
                }
                else
                {
                    if (i == list.Count - 1)//последняя
                    {
                        yield return new DateTimeRange(begin.Begin, list[i].End);
                    }
                }
            }

        }
        public static IEnumerable<DateTimeRange> Slice(this IEnumerable<DateTimeRange> slices)
        {
            IEnumerable<DateTime> dateTimes()
            {
                foreach (var x in slices)
                {
                    yield return x.Begin;
                    yield return x.End;
                }
            }
            var data = dateTimes().Distinct().OrderBy(x => x).ToList();
            if (data.Count < 2)
            {
                yield break;
            }
            for (var i = 1; i < data.Count; i++)
            {
                yield return new DateTimeRange(data[i - 1], data[i]);
            }
        }
        public static IEnumerable<DateTimeRange> Intersect(this DateTimeRange x, params DateTimeRange[] ranges)
        {
            if (ranges == null)
            {
                yield return x;
            }
            else
            {
                foreach (var r in x.Intersect(ranges.AsEnumerable()))
                {
                    yield return r;
                }
            }
        }
        public static IEnumerable<DateTimeRange> Intersect(this DateTimeRange x, IEnumerable<DateTimeRange> ranges)
        {
            if (ranges == null)
            {
                yield return x;
            }
            else
            {
                foreach (var r in ranges)
                {
                    if (r.Begin >= x.Begin && r.Begin < x.End)
                    {
                        yield return new DateTimeRange(
                            r.Begin,
                            x.End < r.End ? x.End : r.End
                            );
                    }
                }
            }
        }
        public static IEnumerable<DateTimeRange> Except(this DateTimeRange x, params DateTimeRange[] ranges)
        {
            if (ranges == null)
            {
                yield return x;
            }
            else
            {
                foreach (var t in x.Except(ranges.AsEnumerable()))
                {
                    yield return t;
                }
            }
        }
        public static IEnumerable<DateTimeRange> Except2(this DateTimeRange x, IEnumerable<DateTimeRange> ranges)
        {
            if (ranges == null || !ranges.Any())
            {
                yield return x;
            }
            var last = ranges.OrderBy(x => x.End).ToList();
            var tmp = ranges.OrderBy(x => x.Begin).ToList();
            foreach (var ls in last)
            {
                foreach (var t in tmp)
                {
                    if (ls.End < x.Begin || t.End > x.End)//не войдут в исходный интервал
                    {
                        break;
                    }
                    if (ls.End < t.Begin)//есть пересечение интервалов
                    {
                        break;
                    }
                    yield return new DateTimeRange(ls.End, t.Begin);
                }
            }

        }
        public static IEnumerable<DateTimeRange> Except(this DateTimeRange x, IEnumerable<DateTimeRange> ranges)
        {
            if (ranges == null || !ranges.Any())
            {
                yield return x;
            }
            var last = ranges.OrderBy(x => x.Begin).ThenBy(x => x.End).ToList();
            for (var i = 0; i < last.Count - 1; i++)
            {
                var f = last[i];
                var s = last[i + 1];
                if (f.End < s.Begin && f.End > x.Begin && s.Begin < x.End)
                {
                    yield return new DateTimeRange(f.End, s.Begin);
                }
            }

        }
    }

}
