<template>
  <div class="app-container">
    <!-- $t is vue-i18n global function to translate lang -->
    <el-table :data="list" v-loading.body="listLoading" element-loading-text="拼命加载中" @sort-change="sortChange" @row-dblclick="rowClick"
      stripe border fit highlight-current-row>
      <el-table-column align="center" label='序号' width="65">
        <template slot-scope="scope">
          {{scope.$index+1}}
        </template>
      </el-table-column>
      <el-table-column label="字段名" prop="columnName" sortable="custom" width="200" align="center">
        <template slot-scope="scope">
          {{scope.row.columnName}}
          <el-tag v-if="scope.row.isPrimaryKey">主键</el-tag>
          <el-tag v-if="scope.row.isIdentity">自增</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="中文名" prop="columnCNName" sortable="custom" width="100" align="center">
        <template slot-scope="scope">
          {{scope.row.columnCNName}}
        </template>
      </el-table-column>
      <el-table-column label="数据类型" prop="ColumnType" sortable="custom" :formatter="typeFormat" width="130" align="center">
      </el-table-column>
      <el-table-column label="默认值" prop="DefaultValue" sortable="custom" width="105" align="center">
        <template slot-scope="scope">
          {{scope.row.defaultValue}}
        </template>
      </el-table-column>
      <el-table-column label="空值" prop="IsNullable" sortable="custom" width="65" align="center">
        <template slot-scope="scope">
          <i class="el-icon-success" v-if="scope.row.isNullable"></i>
        </template>
      </el-table-column>
      <el-table-column label="注释" prop="Remark" sortable="custom" width="105" align="center">
        <template slot-scope="scope">
          {{scope.row.remark}}
        </template>
      </el-table-column>
      <el-table-column align="center" :label="$t('table.actions')" width="160" class-name="small-padding fixed-width">
        <template slot-scope="scope">
          <el-button type="primary" size="mini" @click="handleUpdate(scope.row)">{{$t('table.edit')}}</el-button>
          <el-button size="mini" type="danger" @click="handleDelete(scope.row)">{{$t('table.delete')}}
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-pagination background @size-change="handleSizeChange" @current-change="handleCurrentChange" :current-page="currentPage"
      :page-sizes="[10, 20, 30, 40]" :page-size="pageSize" layout="total, sizes, ->,prev, pager, next, jumper" :total="totalCount">
    </el-pagination>
  </div>
</template>

<script>
  import {
    fetchColumnList,
    deleteArticle
  } from "@/api/article";

  export default {
    name: "TableConfigList",
    props: {
      query: {
        type: String,
        default: null
      }
    },
    computed: {
      skipCount: function () {
        return (this.currentPage - 1) * this.pageSize;
      }
    },
    data() {
      return {
        list: null,
        totalCount: 0,
        currentPage: 1,
        pageSize: 10,
        sort: "",
        listLoading: false,
        temp: {
          id: "",
          dbId: -1,
          columnName: "",
          columnCNName: "",
          columnType: "",
          byteLength: -1,
          charLength: -1,
          scale: -1,
          defaultValue: "",
          remark: "",
          isNullable: false,
          isPrimaryKey: false,
          isIdentity: false
        }
      };
    },
    methods: {
      fetchData() {
        this.listLoading = true;
        fetchColumnList({
          SkipCount: this.skipCount,
          MaxResultCount: this.pageSize,
          Sorting: this.sort,
          filterJson: this.query
        }).then(response => {
          this.list = response.data.result.items;
          this.totalCount = response.data.result.totalCount;
          this.listLoading = false;
        });
      },
      handleUpdate(row) {
        this.temp = Object.assign({}, row); // copy obj

        this.$emit("cu-visiable-change", true, "update", this.temp);
      },
      handleDelete(row) {
        this.$confirm("此操作将永久删除该行数据, 是否继续?", "提示", {
            confirmButtonText: "确定",
            cancelButtonText: "取消",
            type: "warning"
          })
          .then(() => {
            this.temp = Object.assign({}, row); // copy obj
            deleteArticle(this.temp).then(() => {
              this.$message({
                type: "success",
                message: "删除成功!"
              });
              this.fetchData();
            });
          })
          .catch(() => {
            this.$message({
              type: "info",
              message: "已取消删除"
            });
          });
      },
      handleSizeChange(val) {
        this.pageSize = val;
        this.fetchData();
      },
      handleCurrentChange(val) {
        this.currentPage = val;
        this.fetchData();
      },
      sortChange(column) {
        var sortBy = column.order == "descending" ? "DESC" : "ASC";
        this.sort = column.prop + " " + sortBy;
        this.fetchData();
      },
      rowClick(row) {
        this.temp = Object.assign({}, row); // copy obj

        this.$emit("detail-visiable-change", true, this.temp);
      },
      typeFormat(row, column) {
        if (row.charLength >= 0) {
          return row.columnType + "(" + row.charLength + ")";
        } else if (row.byteLength >= 0) {
          return (
            row.columnType +
            "(" +
            row.byteLength +
            "," +
            row.scale +
            ")"
          );
        } else {
          return row.columnType;
        }
      }
    }
  };

</script>
