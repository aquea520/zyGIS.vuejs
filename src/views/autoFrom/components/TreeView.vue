<template>
  <div class="treeview-container">
    <el-input placeholder="输入关键字进行过滤" v-model="filterText">
    </el-input>

    <el-tree class="filter-tree" v-loading.body="listLoading"
    :data="list" 
    :props="defaultProps" 
    default-expand-all highlight-current
    :filter-node-method="filterNode" 
    @node-click="handleNodeClick" ref="tree2">
    </el-tree>
  </div>
</template>

<script>
import {
  fetchTreeNode
} from "@/api/article";

export default {
  name: "TreeView",
  watch: {
    filterText(val) {
      this.$refs.tree2.filter(val);
    }
  },

  methods: {
    filterNode(value, data) {
      if (!value) return true;
      return data.label.indexOf(value) !== -1;
    },
    fetchData() {
      this.listLoading = true;
      fetchTreeNode({}).then(response => {
        this.list = response.data.result;
        this.listLoading = false;
      });
    },
    handleNodeClick(data) {
      this.$emit("node-click", data.id);
    }
  },
  created() {
    this.fetchData();
  },
  data() {
    return {
      listLoading: true,
      list : null,
      filterText: "",
      defaultProps: {
        children: "childrens",
        label: "name"
      }
    };
  }
};
</script>

<style rel="stylesheet/scss" lang="scss" scoped>
.treeview-container {
  padding: 20px;
  .chart-wrapper {
    background: #fff;
    padding: 16px 16px 0;
    margin-bottom: 32px;
  }
}

.filter-tree {
  padding-top: 20px;
}
</style>