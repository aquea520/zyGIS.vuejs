import request from '@/utils/request'
import { type } from 'os';

export function fetchColumnList(query) {
  return request({
    url: '/TableConfigService/GetAllByQuery',
    method: 'get',
    params: query
  })
}

export function fetchSchema(query) {
  return request({
    url: '/TableConfigService/GetSchema',
    method: 'get',
    params: query
  })
}

export function fetchTreeNode(query) {
  return request({
    url: '/DataDictionaryService/GetTreeData',
    method: 'get',
    params: query
  })
}

export function fetchModelById(query) {
  return request({
    url: '/TableConfigService/Get',
    method: 'get',
    params: query
  })
}

export function fetchList(query) {
  return request({
    url: '/article/list',
    method: 'get',
    params: query
  })
}

export function fetchArticle() {
  return request({
    url: '/article/detail',
    method: 'get'
  })
}

export function fetchPv(pv) {
  return request({
    url: '/article/pv',
    method: 'get',
    params: { pv }
  })
}

export function createArticle(data) {
  return request({
    url: '/TableConfigService/Create',
    method: 'post',
    data : data
  })
}

export function updateArticle(data) {
  return request({
    url: '/TableConfigService/Update',
    method: 'put',
    data : data
  })
}

export function deleteArticle(id) {
  return request({
    url: '/TableConfigService/Delete',
    method: 'delete',
    params : id
  })
}
