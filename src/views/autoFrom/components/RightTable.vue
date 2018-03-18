<template>
  <div class="app-container">
    <!-- $t is vue-i18n global function to translate lang -->
    <TableConfigQuery ref="query" :treeNodeId = "treeNodeId" v-on:table-fetchData="OnTableFetchData" v-on:cu-visiable-change="OnCUVisiableChange"></TableConfigQuery>

    <TableConfigList ref="list" :query="query" v-on:cu-visiable-change="OnCUVisiableChange" v-on:detail-visiable-change="OnDetailVisiableChange"></TableConfigList>

    <CreateOrUpdate ref="create" :formId="temp.id" :formPId="treeNodeId" :formVisiable="dialogFormVisible" :formStatus="dialogStatus" v-on:cu-visiable-change="OnCUVisiableChange"></CreateOrUpdate>

    <Detail :formId="temp.id" :detailVisiable="detailVisiable" v-on:detail-visiable-change="OnDetailVisiableChange"></Detail>

  </div>
</template>

<script>
import {
  fetchColumnList,
  createArticle,
  updateArticle,
  deleteArticle,
  fetchSchema
} from "@/api/article";

import { default as CreateOrUpdate } from "./CreateOrUpdate";
import { default as Detail } from "./Detail";
import { default as TableConfigList } from "./TableConfigList";
import { default as TableConfigQuery } from "./TableConfigQuery";

export default {
  name: "RightTable",
  components: {
    CreateOrUpdate,
    Detail,
    TableConfigQuery,
    TableConfigList
  },
   props: {
    treeNodeId: {
      type: String,
      default: null
    }
  },
  data() {
    return {
      query: "",
      dialogFormVisible: false,
      dialogStatus: "",
      detailVisiable: false,
      temp: {
        id: "",
        dbId: this.treeNodeId,
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
    OnCUVisiableChange: function(visiable, status, row) {
      this.dialogFormVisible = visiable;
      this.dialogStatus = status;
      if (row != null) {
        this.temp = row;
      }

      if (!visiable && status == "") {
        this.$refs["list"].fetchData();
      }
    },
    OnDetailVisiableChange: function(visiable, row) {
      this.detailVisiable = visiable;
      if (row != null) {
        this.temp = row;
      }
    },
    OnTableFetchData: function(queryStr) {
      this.query = queryStr;
      this.$nextTick(() => {
        this.$refs["list"].fetchData();
      });
    }
  }
};
</script>