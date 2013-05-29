using System;
using System.Data;

namespace Clifton.Tools.Data
{
	public static class RawDataTable
	{
		public static void Serialize(RawSerializer rs, DataTable dt)
		{
			rs.Serialize(dt.TableName);
			rs.Serialize(dt.Columns.Count);
			rs.Serialize(dt.Rows.Count);
			rs.Serialize(dt.PrimaryKey.Length);

			foreach (DataColumn dc in dt.PrimaryKey)
			{
				rs.Serialize(dc.ColumnName);
			}

			foreach (DataColumn dc in dt.Columns)
			{
				rs.Serialize(dc.ColumnName);
				rs.Serialize(dc.AllowDBNull);
				rs.Serialize(dc.DataType.AssemblyQualifiedName);
			}

			foreach (DataRow dr in dt.Rows)
			{
				foreach (DataColumn dc in dt.Columns)
				{
					if (dc.AllowDBNull)
					{
						rs.SerializeNullable(dr[dc]);
					}
					else
					{
						rs.Serialize(dr[dc]);
					}
				}
			}
		}

		public static DataTable Deserialize(RawDeserializer rd)
		{
			string tableName = rd.DeserializeString();
			int columns = rd.DeserializeInt();
			int rows = rd.DeserializeInt();
			int pkCount = rd.DeserializeInt();
			string[] pkColNames=null;

			// Read the PK column names.
			if (pkCount > 0)
			{
				pkColNames = new string[pkCount];

				for (int i = 0; i < pkCount; i++)
				{
					pkColNames[i] = rd.DeserializeString();
				}
			}

			DataTable dtIn = new DataTable(tableName);

			// Initialize the columns.
			for (int x = 0; x < columns; x++)
			{
				string columnName = rd.DeserializeString();
				bool allowNulls = rd.DeserializeBool();
				string type = rd.DeserializeString();

				DataColumn dc = new DataColumn(columnName, Type.GetType(type));
				dc.AllowDBNull = allowNulls;
				dtIn.Columns.Add(dc);
			}

			if (pkCount > 0)
			{
				DataColumn[] pkCols = new DataColumn[pkCount];

				for (int i = 0; i < pkCount; i++)
				{
					pkCols[i] = dtIn.Columns[pkColNames[i]];
				}

				dtIn.PrimaryKey = pkCols;
			}

			// Initialize the rows.
			for (int y = 0; y < rows; y++)
			{
				DataRow dr = dtIn.NewRow();

				for (int x = 0; x < columns; x++)
				{
					DataColumn dc = dtIn.Columns[x];
					object obj;

					if (dc.AllowDBNull)
					{
						obj = rd.DeserializeNullable(dc.DataType);
					}
					else
					{
						obj = rd.Deserialize(dc.DataType);
					}

					dr[dc] = obj;
				}

				dtIn.Rows.Add(dr);
			}

			return dtIn;
		}
	}
}
