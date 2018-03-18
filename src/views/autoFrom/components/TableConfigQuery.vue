<template>
  <div class="app-container">
    <!-- $t is vue-i18n global function to translate lang -->
    <el-input style='width:300px;' placeholder="请输入字段名称" prefix-icon="el-icon-document" v-model="clnname"></el-input>
    <el-button style='margin-bottom:20px;' type="primary" icon="document" :loading="downloadLoading" @click="fetchData()">{{$t('table.search')}}</el-button>
    <el-button class="filter-item" style="margin-left: 10px;" @click="handleCreate" type="primary" icon="el-icon-edit">{{$t('table.add')}}</el-button>
  </div>
</template>

<script>
  import {
    fetchColumnList,
    createArticle,
    updateArticle,
    deleteArticle
  } from "@/api/article";

  export default {
    name: "TableConfigQuery",
    props: {
      treeNodeId: {
        type: String,
        default: null
      }
    },
    data() {
      return {
        downloadLoading: false,
        clnname: ""
      };
    },
    methods: {
      handleCreate() {
        this.$emit("cu-visiable-change", true, "create", null);
      },
      fetchData() {
        var group = {
          rules: [],
          op: "and"
        };
        if (this.clnname != "") {
          group.rules.push({
            field: "ColumnName",
            op: "like",
            value: this.clnname,
            type: "string"
          });
        }

        if (this.treeNodeId && this.treeNodeId != "") {
          group.rules.push({
            field: "DBId",
            op: "equal",
            value: this.treeNodeId,
            type: "string"
          });
        }

        this.$emit("table-fetchData", JSON.stringify(group));
      }
    }
  };

</script>
