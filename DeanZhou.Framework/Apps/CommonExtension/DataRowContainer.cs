using System;
using System.ComponentModel;
using System.Data;

namespace DeanZhou.Framework
{
    public class DataRowContainer
    {
        public DataRow DataRow { get; set; }

        public DataRowContainer()
        { }

        public DataRowContainer(DataRow row)
        {
            DataRow = row;
        }

        public void InitRow(DataRow row)
        {
            DataRow = row;
        }

        /// <summary>
        /// 获取一个值，该值指示某行是否包含错误。
        /// </summary>
        public bool HasErrors { get { return DataRow.HasErrors; } }

        /// <summary>
        /// 通过一个数组来获取或设置此行的所有值。
        /// </summary>
        public object[] ItemArray { get { return DataRow.ItemArray; } set { DataRow.ItemArray = value; } }

        /// <summary>
        /// 获取或设置行的自定义错误说明
        /// </summary>
        public string RowError { get { return DataRow.RowError; } set { DataRow.RowError = value; } }

        /// <summary>
        /// 获取与该行和 System.Data.DataRowCollection 的关系相关的当前状态。
        /// </summary>
        public DataRowState RowState { get { return DataRow.RowState; } }

        /// <summary>
        /// 获取该行拥有其架构的 System.Data.DataTable。
        /// </summary>
        public DataTable Table { get { return DataRow.Table; } }

        /// <summary>
        /// 获取或设置存储在指定的 System.Data.DataColumn 中的数据。
        /// </summary>
        /// <param name="column">System.Data.DataColumn，包含该数据。</param>
        /// <returns> System.Object，包含该数据。</returns>
        public object this[DataColumn column] { get { return DataRow[column]; } }

        /// <summary>
        /// 获取或设置存储在由索引指定的列中的数据
        /// </summary>
        /// <param name="columnIndex">列的从零开始的索引</param>
        /// <returns>System.Object，包含该数据</returns>
        public object this[int columnIndex] { get { return DataRow[columnIndex]; } set { DataRow[columnIndex] = value; } }

        /// <summary>
        /// 获取或设置存储在由名称指定的列中的数据
        /// </summary>
        /// <param name="columnName">列的名称</param>
        /// <returns>System.Object，包含该数据</returns>
        public object this[string columnName] { get { return DataRow[columnName]; } set { DataRow[columnName] = value; } }

        /// <summary>
        /// 获取或设置存储在由名称指定的列中的数据
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <param name="isBool">此列是否Bool列（主要是解决firebird和Sql中Bool列的问题）</param>
        /// <returns>System.Object，包含该数据</returns>
        public object this[int columnIndex, bool isBool]
        {
            get
            {
                object obj = DataRow[columnIndex];
                if (isBool)
                {
                    if (obj is Boolean)
                    {
                        return (bool)DataRow[columnIndex];
                    }
                    return !obj.ToString().Equals("0");
                }
                return obj;
            }
            set { DataRow[columnIndex] = value; }
        }

        /// <summary>
        /// 获取或设置存储在由名称指定的列中的数据
        /// </summary>
        /// <param name="columnName">列的名称</param>
        /// <param name="isBool">此列是否Bool列（主要是解决firebird和Sql中Bool列的问题）</param>
        /// <returns>System.Object，包含该数据</returns>
        public object this[string columnName, bool isBool]
        {
            get
            {
                object obj = DataRow[columnName];
                if (isBool)
                {
                    if (obj is Boolean)
                    {
                        return (bool)DataRow[columnName];
                    }
                    return !obj.ToString().Equals("0");
                }
                return obj;
            }
            set { DataRow[columnName] = value; }
        }

        /// <summary>
        /// 获取存储在指定的 System.Data.DataColumn 中的数据的指定版本
        /// </summary>
        /// <param name="column">System.Data.DataColumn，包含有关该列的信息</param>
        /// <param name="version">System.Data.DataRowVersion 值之一，用于指定您需要的行版本。可能值为 Default、Original、Current和 Proposed</param>
        /// <returns>System.Object，包含该数据</returns>
        public object this[DataColumn column, DataRowVersion version] { get { return DataRow[column, version]; } }

        /// <summary>
        /// 获取存储在由索引和要检索的数据的版本指定的列中的数据
        /// </summary>
        /// <param name="columnIndex">列的从零开始的索引</param>
        /// <param name="version">System.Data.DataRowVersion 值之一，用于指定您需要的行版本。可能值为 Default、Original、Current和 Proposed</param>
        /// <returns>System.Object，包含该数据</returns>
        public object this[int columnIndex, DataRowVersion version] { get { return DataRow[columnIndex, version]; } }

        /// <summary>
        /// 获取存储在已命名列中的数据的指定版本
        /// </summary>
        /// <param name="columnName">列的名称</param>
        /// <param name="version">System.Data.DataRowVersion 值之一，用于指定您需要的行版本。可能值为 Default、Original、Current和 Proposed</param>
        /// <returns>System.Object，包含该数据</returns>
        public object this[string columnName, DataRowVersion version] { get { return DataRow[columnName, version]; } }

        /// <summary>
        /// 提交自上次调用 System.Data.DataRow.AcceptChanges() 以来对该行进行的所有更改
        /// </summary>
        public void AcceptChanges()
        {
            DataRow.AcceptChanges();
        }

        /// <summary>
        /// 对 System.Data.DataRow 对象开始编辑操作
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void BeginEdit()
        {
            DataRow.BeginEdit();
        }

        /// <summary>
        /// 取消对该行的当前编辑
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void CancelEdit()
        {
            DataRow.CancelEdit();
        }

        /// <summary>
        /// 清除行的错误。这包括 System.Data.DataRow.RowError 和用 System.Data.DataRow.SetColumnError(System.Int32,System.String)设置的错误
        /// </summary>
        public void ClearErrors()
        {
            DataRow.ClearErrors();
        }

        /// <summary>
        /// 删除 System.Data.DataRow
        /// </summary>
        public void Delete()
        {
            DataRow.Delete();
        }

        /// <summary>
        /// 终止发生在该行的编辑
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void EndEdit()
        {
            DataRow.EndEdit();
        }

        /// <summary>
        /// 使用指定的 System.Data.DataRelation 获取此 System.Data.DataRow 的子行
        /// </summary>
        /// <param name="relation">要使用的 System.Data.DataRelation</param>
        /// <returns>System.Data.DataRow 对象的数组，或长度为零的数组</returns>
        public DataRow[] GetChildRows(DataRelation relation)
        {
            return DataRow.GetChildRows(relation);
        }

        /// <summary>
        /// 使用 System.Data.DataRelation 的指定 System.Data.DataRelation.RelationName 获取System.Data.DataRow 的子行
        /// </summary>
        /// <param name="relationName">要使用的 System.Data.DataRelation 的 System.Data.DataRelation.RelationName</param>
        /// <returns>System.Data.DataRow 对象的数组，或长度为零的数组</returns>
        public DataRow[] GetChildRows(string relationName)
        {
            return DataRow.GetChildRows(relationName);
        }

        /// <summary>
        /// 使用指定的 System.Data.DataRelation 和 System.Data.DataRowVersion 获取 System.Data.DataRow的子行
        /// </summary>
        /// <param name="relation">要使用的 System.Data.DataRelation</param>
        /// <param name="version">System.Data.DataRowVersion 值之一，它指定要获取的数据的版本。可能值为 Default、Original、Current和 Proposed</param>
        /// <returns>System.Data.DataRow 对象的数组</returns>
        public DataRow[] GetChildRows(DataRelation relation, DataRowVersion version)
        {
            return DataRow.GetChildRows(relation, version);
        }

        /// <summary>
        /// 使用 System.Data.DataRelation 的指定 System.Data.DataRelation.RelationName 和 System.Data.DataRowVersion获取 System.Data.DataRow 的子行
        /// </summary>
        /// <param name="relationName">要使用的 System.Data.DataRelation 的 System.Data.DataRelation.RelationName</param>
        /// <param name="version">System.Data.DataRowVersion 值之一，它指定要获取的数据的版本。可能值为 Default、Original、Current和 Proposed</param>
        /// <returns>System.Data.DataRow 对象的数组，或长度为零的数组</returns>
        public DataRow[] GetChildRows(string relationName, DataRowVersion version)
        {
            return DataRow.GetChildRows(relationName, version);
        }

        /// <summary>
        /// 获取指定的 System.Data.DataColumn 的错误说明
        /// </summary>
        /// <param name="column">System.Data.DataColumn</param>
        /// <returns>错误说明的文本</returns>
        public string GetColumnError(DataColumn column)
        {
            return DataRow.GetColumnError(column);
        }

        /// <summary>
        /// 获取由索引指定的列的错误说明
        /// </summary>
        /// <param name="columnIndex">列的从零开始的索引</param>
        /// <returns>错误说明的文本</returns>
        public string GetColumnError(int columnIndex)
        {
            return DataRow.GetColumnError(columnIndex);
        }

        /// <summary>
        /// 获取由名称指定的列的错误说明
        /// </summary>
        /// <param name="columnName">列的名称</param>
        /// <returns>错误说明的文本</returns>
        public string GetColumnError(string columnName)
        {
            return DataRow.GetColumnError(columnName);
        }

        /// <summary>
        /// 获取包含错误的列的数组
        /// </summary>
        /// <returns>包含错误的 System.Data.DataColumn 对象的数组</returns>
        public DataColumn[] GetColumnsInError()
        {
            return DataRow.GetColumnsInError();
        }

        /// <summary>
        /// 使用指定的 System.Data.DataRelation 获取 System.Data.DataRow 的父行
        /// </summary>
        /// <param name="relation">要使用的 System.Data.DataRelation</param>
        /// <returns>当前行的父级 System.Data.DataRow</returns>
        public DataRow GetParentRow(DataRelation relation)
        {
            return DataRow.GetParentRow(relation);
        }

        /// <summary>
        /// 使用 System.Data.DataRelation 的指定 System.Data.DataRelation.RelationName 获取System.Data.DataRow 的父行
        /// </summary>
        /// <param name="relationName">System.Data.DataRelation 的 System.Data.DataRelation.RelationName</param>
        /// <returns>当前行的父级 System.Data.DataRow</returns>
        public DataRow GetParentRow(string relationName)
        {
            return DataRow.GetParentRow(relationName);
        }

        /// <summary>
        /// 使用指定的 System.Data.DataRelation 和 System.Data.DataRowVersion 获取 System.Data.DataRow的父行
        /// </summary>
        /// <param name="relation">要使用的 System.Data.DataRelation</param>
        /// <param name="version">System.Data.DataRowVersion 值之一，它指定要获取的数据的版本</param>
        /// <returns>当前行的父级 System.Data.DataRow</returns>
        public DataRow GetParentRow(DataRelation relation, DataRowVersion version)
        {
            return DataRow.GetParentRow(relation, version);
        }

        /// <summary>
        /// 使用 System.Data.DataRelation 的指定 System.Data.DataRelation.RelationName 和 System.Data.DataRowVersion获取 System.Data.DataRow 的父行
        /// </summary>
        /// <param name="relationName">System.Data.DataRelation 的 System.Data.DataRelation.RelationName</param>
        /// <param name="version">System.Data.DataRowVersion 值之一</param>
        /// <returns>当前行的父级 System.Data.DataRow</returns>
        public DataRow GetParentRow(string relationName, DataRowVersion version)
        {
            return DataRow.GetParentRow(relationName, version);
        }

        /// <summary>
        /// 使用指定的 System.Data.DataRelation 获取 System.Data.DataRow 的父行
        /// </summary>
        /// <param name="relation">要使用的 System.Data.DataRelation</param>
        /// <returns>System.Data.DataRow 对象的数组，或长度为零的数组</returns>
        public DataRow[] GetParentRows(DataRelation relation)
        {
            return DataRow.GetParentRows(relation);
        }

        /// <summary>
        /// 使用 System.Data.DataRelation 的指定 System.Data.DataRelation.RelationName 获取System.Data.DataRow 的父行
        /// </summary>
        /// <param name="relationName">System.Data.DataRelation 的 System.Data.DataRelation.RelationName</param>
        /// <returns>System.Data.DataRow 对象的数组，或长度为零的数组</returns>
        public DataRow[] GetParentRows(string relationName)
        {
            return DataRow.GetParentRows(relationName);
        }

        /// <summary>
        /// 使用指定的 System.Data.DataRelation 和 System.Data.DataRowVersion 获取 System.Data.DataRow的父行
        /// </summary>
        /// <param name="relation">要使用的 System.Data.DataRelation</param>
        /// <param name="version">System.Data.DataRowVersion 值之一，它指定要获取的数据的版本</param>
        /// <returns>System.Data.DataRow 对象的数组，或长度为零的数组</returns>
        public DataRow[] GetParentRows(DataRelation relation, DataRowVersion version)
        {
            return DataRow.GetParentRows(relation, version);
        }

        /// <summary>
        /// 使用 System.Data.DataRelation 的指定 System.Data.DataRelation.RelationName 和 System.Data.DataRowVersion获取 System.Data.DataRow 的父行
        /// </summary>
        /// <param name="relationName">System.Data.DataRelation 的 System.Data.DataRelation.RelationName和 Proposed</param>
        /// <param name="version">System.Data.DataRowVersion 值之一，它指定要获取的数据的版本。可能值为 Default、Original、Current</param>
        /// <returns>System.Data.DataRow 对象的数组，或长度为零的数组</returns>
        public DataRow[] GetParentRows(string relationName, DataRowVersion version)
        {
            return DataRow.GetParentRows(relationName, version);
        }

        /// <summary>
        /// 获取一个值，该值指示指定的版本是否存在
        /// </summary>
        /// <param name="version">System.Data.DataRowVersion 值之一，它指定行版本</param>
        /// <returns>如果版本存在，则为 true；否则为 false</returns>
        public bool HasVersion(DataRowVersion version)
        {
            return DataRow.HasVersion(version);
        }

        /// <summary>
        /// 获取一个值，该值指示指定的 System.Data.DataColumn 是否包含 null 值
        /// </summary>
        /// <param name="column">System.Data.DataColumn</param>
        /// <returns>如果列不存在或列包含 null 值，则为 true；否则为 false</returns>
        public bool IsNull(DataColumn column)
        {
            if (DataRow == null) return false;
            if (column == null)
            {
                return true;
            }
            return DataRow.IsNull(column);
        }

        /// <summary>
        /// 获取一个值，该值指示指定的列是否包含 null 值
        /// </summary>
        /// <returns>如果列不存在或列包含 null 值，则为 true；否则为 false</returns>
        public bool IsNull(int columnIndex)
        {
            if (DataRow == null) return false;
            if (columnIndex >= DataRow.Table.Columns.Count)
            {
                return true;
            }
            return DataRow.IsNull(columnIndex);
        }

        /// <summary>
        /// 获取一个值，该值指示指定的列是否包含 null 值
        /// </summary>
        /// <param name="columnName">列的名称</param>
        /// <returns>如果列不存在或列包含 null 值，则为 true；否则为 false</returns>
        public bool IsNull(string columnName)
        {
            try
            {
                return DataRow.IsNull(columnName);
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// 获取一个值，该值指示指定的 System.Data.DataColumn 和 System.Data.DataRowVersion 是否包含 null值
        /// </summary>
        /// <param name="column">System.Data.DataColumn</param>
        /// <param name="version">System.Data.DataRowVersion 值之一，它指定行版本。可能值为 Default、Original、Current 和 Proposed</param>
        /// <returns>如果列包含 null 值，则为 true；否则为 false</returns>
        public bool IsNull(DataColumn column, DataRowVersion version)
        {
            if (DataRow == null) return false;
            if (column == null)
            {
                return true;
            }
            return DataRow.IsNull(column, version);
        }

        /// <summary>
        /// 拒绝自上次调用 System.Data.DataRow.AcceptChanges() 以来对该行进行的所有更改
        /// </summary>
        public void RejectChanges()
        {
            DataRow.RejectChanges();
        }

        /// <summary>
        /// 将 System.Data.DataRow 的 System.Data.DataRow.Rowstate 更改为 Added
        /// </summary>
        public void SetAdded()
        {
            DataRow.SetAdded();
        }

        /// <summary>
        /// 为指定为 System.Data.DataColumn 的列设置错误说明
        /// </summary>
        /// <param name="column">要为其设置错误说明的 System.Data.DataColumn</param>
        /// <param name="error">错误说明</param>
        public void SetColumnError(DataColumn column, string error)
        {
            DataRow.SetColumnError(column, error);
        }

        /// <summary>
        /// 为由索引指定的列设置错误说明
        /// </summary>
        /// <param name="columnIndex">列的从零开始的索引</param>
        /// <param name="error">错误说明</param>
        public void SetColumnError(int columnIndex, string error)
        {
            DataRow.SetColumnError(columnIndex, error);
        }

        /// <summary>
        /// 为由名称指定的列设置错误说明
        /// </summary>
        /// <param name="columnName">列的名称</param>
        /// <param name="error">错误说明</param>
        public void SetColumnError(string columnName, string error)
        {
            DataRow.SetColumnError(columnName, error);
        }

        /// <summary>
        /// 将 System.Data.DataRow 的 System.Data.DataRow.Rowstate 更改为 Modified
        /// </summary>
        public void SetModified()
        {
            DataRow.SetModified();
        }

        /// <summary>
        /// 用新指定的父级 System.Data.DataRow 设置 System.Data.DataRow 的父行
        /// </summary>
        /// <param name="parentRow">新的父级 System.Data.DataRow</param>
        public void SetParentRow(DataRow parentRow)
        {
            DataRow.SetParentRow(parentRow);
        }

        /// <summary>
        /// 用新指定的父级 System.Data.DataRow 和 System.Data.DataRelation 设置 System.Data.DataRow的父行
        /// </summary>
        /// <param name="parentRow">新的父级 System.Data.DataRow</param>
        /// <param name="relation">要使用的 System.Data.DataRelation 关系</param>
        public void SetParentRow(DataRow parentRow, DataRelation relation)
        {
            DataRow.SetParentRow(parentRow, relation);
        }

        /// <summary>
        /// 判断数据行中是否有给定名称的列
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <returns>true/false</returns>
        public bool HasColumn(string columnName)
        {
            if (DataRow == null) return false;
            return DataRow.Table.Columns.Contains(columnName);
        }

        /// <summary>
        /// 从行中获取给定列的字符值
        /// </summary>
        public char Char(string columnName)
        {
            return Char(columnName, ' ');
        }

        /// <summary>
        /// 从行中获取给定列的字符值，如果为空，则取给定的默认值
        /// </summary>
        public char Char(string columnName, char defaultValue)
        {
            string tmp = String(columnName);
            if (!string.IsNullOrEmpty(tmp))
            {
                return tmp[0];
            }
            return defaultValue;
        }

        /// <summary>
        /// 从行中获取给定列的字符串值
        /// </summary>
        public string String(string columnName)
        {
            return String(columnName, string.Empty);
        }

        /// <summary>
        /// 从行中获取给定列的字符串值，如果为空，则取给定的默认值
        /// </summary>
        public string String(string columnName, string defaultValue)
        {
            return IsNull(columnName) ? defaultValue : Convert.ToString(this[columnName]);
        }

        /// <summary>
        /// 从行中获取给定列的8位无符号整数
        /// </summary>
        public byte Byte(string columnName)
        {
            return Byte(columnName, Null.NullByte);
        }

        /// <summary>
        /// 位无符号整数，如果为空，则取给定的默认值
        /// </summary>
        public byte Byte(string columnName, byte defaultValue)
        {
            try
            {
                return IsNull(columnName) ? defaultValue : Convert.ToByte(this[columnName]);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 从行中获取给定列的16位整数值
        /// </summary>
        public short Short(string columnName)
        {
            return Short(columnName, Null.NullShort);
        }

        /// <summary>
        /// 从行中获取给定列的16位整数值，如果为空，则取给定的默认值
        /// </summary>
        public short Short(string columnName, short defaultValue)
        {
            try
            {
                return IsNull(columnName) ? defaultValue : Convert.ToInt16(this[columnName]);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 从行中获取给定列的整数值
        /// </summary>
        public int Int(string columnName)
        {
            return Int(columnName, Null.NullInteger);
        }

        /// <summary>
        /// 从行中获取给定列的整数值，如果为空，则取给定的默认值
        /// </summary>
        public int Int(string columnName, int defaultValue)
        {
            try
            {
                return IsNull(columnName) ? defaultValue : Convert.ToInt32(this[columnName]);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 从行中获取给定列的长整数值
        /// </summary>
        public long Long(string columnName)
        {
            return Long(columnName, Null.NullLong);
        }

        /// <summary>
        /// 从行中获取给定列的长整数值，如果为空，则取给定的默认值
        /// </summary>
        public long Long(string columnName, long defaultValue)
        {
            try
            {
                return IsNull(columnName) ? defaultValue : Convert.ToInt64(this[columnName]);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 从行中获取给定列的十进制数
        /// </summary>
        public decimal Decimal(string columnName)
        {
            return Decimal(columnName, Null.NullDecimal);
        }

        /// <summary>
        /// 从行中获取给定列的十进制数，如果为空，则取给定的默认值
        /// </summary>
        public decimal Decimal(string columnName, decimal defaultValue)
        {
            try
            {
                return IsNull(columnName) ? defaultValue : Convert.ToDecimal(this[columnName]);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 从行中获取给定列的双浮点数
        /// </summary>
        public double Double(string columnName)
        {
            return Double(columnName, Null.NullDouble);
        }

        /// <summary>
        /// 从行中获取给定列的双浮点数，如果为空，则取给定的默认值
        /// </summary>
        public double Double(string columnName, double defaultValue)
        {
            try
            {
                return IsNull(columnName) ? defaultValue : Convert.ToDouble(this[columnName]);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 从行中获取给定列的浮点数
        /// </summary>
        public float Float(string columnName)
        {
            return Float(columnName, Null.NullSingle);
        }

        /// <summary>
        /// 从行中获取给定列的浮点数，如果为空，则取给定的默认值
        /// </summary>
        public float Float(string columnName, float defaultValue)
        {
            try
            {
                return IsNull(columnName) ? defaultValue : Convert.ToSingle(this[columnName]);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 从行中获取给定列的布尔值
        /// </summary>
        public bool Bool(string columnName)
        {
            return Bool(columnName, Null.NullBoolean);
        }

        /// <summary>
        /// 从行中获取给定列的布尔值
        /// </summary>
        public bool Bool(string columnName, bool defaultValue)
        {
            try
            {
                return IsNull(columnName) ? defaultValue : (bool)this[columnName, true];
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 从行中获取给定列的布尔值
        /// </summary>
        public DateTime DateTime(string columnName)
        {
            return DateTime(columnName, Null.NullDate);
        }

        /// <summary>
        /// 从行中获取给定列的布尔值
        /// </summary>
        public DateTime DateTime(string columnName, DateTime defaultValue)
        {
            try
            {
                return IsNull(columnName) ? defaultValue : Convert.ToDateTime(this[columnName]);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 从行中获取给定列的枚举值
        /// </summary>
        public T Enum<T>(string columnName)
        {
            return Enum(columnName, default(T));
        }

        /// <summary>
        /// 从行中获取给定列的枚举值，如果为空，则取给定的默认值
        /// </summary>
        public T Enum<T>(string columnName, T defaultValue)
        {
            try
            {
                return IsNull(columnName) ? defaultValue : (T)this[columnName];
            }
            catch
            {
                return defaultValue;
            }
        }


        /// <summary>
        /// 从行中获取给定列的字符串值
        /// </summary>
        public string String(int ccolumnIndex)
        {
            return String(ccolumnIndex, string.Empty);
        }

        /// <summary>
        /// 从行中获取给定列的字符串值，如果为空，则取给定的默认值
        /// </summary>
        public string String(int ccolumnIndex, string defaultValue)
        {
            return IsNull(ccolumnIndex) ? defaultValue : Convert.ToString(this[ccolumnIndex]);
        }

        /// <summary>
        /// 从行中获取给定列的8位无符号整数
        /// </summary>
        public byte Byte(int ccolumnIndex)
        {
            return Byte(ccolumnIndex, Null.NullByte);
        }

        /// <summary>
        /// 位无符号整数，如果为空，则取给定的默认值
        /// </summary>
        public byte Byte(int ccolumnIndex, byte defaultValue)
        {
            try
            {
                return IsNull(ccolumnIndex) ? defaultValue : Convert.ToByte(this[ccolumnIndex]);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 从行中获取给定列的16位整数值
        /// </summary>
        public short Short(int ccolumnIndex)
        {
            return Short(ccolumnIndex, Null.NullShort);
        }

        /// <summary>
        /// 从行中获取给定列的16位整数值，如果为空，则取给定的默认值
        /// </summary>
        public short Short(int ccolumnIndex, short defaultValue)
        {
            try
            {
                return IsNull(ccolumnIndex) ? defaultValue : Convert.ToInt16(this[ccolumnIndex]);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 从行中获取给定列的整数值
        /// </summary>
        public int Int(int ccolumnIndex)
        {
            return Int(ccolumnIndex, Null.NullInteger);
        }

        /// <summary>
        /// 从行中获取给定列的整数值，如果为空，则取给定的默认值
        /// </summary>
        public int Int(int ccolumnIndex, int defaultValue)
        {
            try
            {
                return IsNull(ccolumnIndex) ? defaultValue : Convert.ToInt32(this[ccolumnIndex]);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 从行中获取给定列的长整数值
        /// </summary>
        public long Long(int ccolumnIndex)
        {
            return Long(ccolumnIndex, Null.NullLong);
        }

        /// <summary>
        /// 从行中获取给定列的长整数值，如果为空，则取给定的默认值
        /// </summary>
        public long Long(int ccolumnIndex, long defaultValue)
        {
            try
            {
                return IsNull(ccolumnIndex) ? defaultValue : Convert.ToInt64(this[ccolumnIndex]);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 从行中获取给定列的十进制数
        /// </summary>
        public decimal Decimal(int ccolumnIndex)
        {
            return Decimal(ccolumnIndex, Null.NullDecimal);
        }

        /// <summary>
        /// 从行中获取给定列的十进制数，如果为空，则取给定的默认值
        /// </summary>
        public decimal Decimal(int ccolumnIndex, decimal defaultValue)
        {
            try
            {
                return IsNull(ccolumnIndex) ? defaultValue : Convert.ToDecimal(this[ccolumnIndex]);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 从行中获取给定列的双浮点数
        /// </summary>
        public double Double(int ccolumnIndex)
        {
            return Double(ccolumnIndex, Null.NullDouble);
        }

        /// <summary>
        /// 从行中获取给定列的双浮点数，如果为空，则取给定的默认值
        /// </summary>
        public double Double(int ccolumnIndex, double defaultValue)
        {
            try
            {
                return IsNull(ccolumnIndex) ? defaultValue : Convert.ToDouble(this[ccolumnIndex]);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 从行中获取给定列的浮点数
        /// </summary>
        public float Float(int ccolumnIndex)
        {
            return Float(ccolumnIndex, Null.NullSingle);
        }

        /// <summary>
        /// 从行中获取给定列的浮点数，如果为空，则取给定的默认值
        /// </summary>
        public float Float(int ccolumnIndex, float defaultValue)
        {
            try
            {
                return IsNull(ccolumnIndex) ? defaultValue : Convert.ToSingle(this[ccolumnIndex]);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 从行中获取给定列的布尔值
        /// </summary>
        public bool Bool(int ccolumnIndex)
        {
            return Bool(ccolumnIndex, Null.NullBoolean);
        }

        /// <summary>
        /// 从行中获取给定列的布尔值
        /// </summary>
        public bool Bool(int ccolumnIndex, bool defaultValue)
        {
            try
            {
                return IsNull(ccolumnIndex) ? defaultValue : (bool)this[ccolumnIndex, true];
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 从行中获取给定列的布尔值
        /// </summary>
        public DateTime DateTime(int ccolumnIndex)
        {
            return DateTime(ccolumnIndex, Null.NullDate);
        }

        /// <summary>
        /// 从行中获取给定列的布尔值
        /// </summary>
        public DateTime DateTime(int ccolumnIndex, DateTime defaultValue)
        {
            try
            {
                return IsNull(ccolumnIndex) ? defaultValue : Convert.ToDateTime(this[ccolumnIndex]);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 从行中获取给定列的枚举值
        /// </summary>
        public T Enum<T>(int ccolumnIndex)
        {
            return Enum(ccolumnIndex, default(T));
        }

        /// <summary>
        /// 从行中获取给定列的枚举值，如果为空，则取给定的默认值
        /// </summary>
        public T Enum<T>(int ccolumnIndex, T defaultValue)
        {
            try
            {
                return IsNull(ccolumnIndex) ? defaultValue : (T)this[ccolumnIndex];
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
