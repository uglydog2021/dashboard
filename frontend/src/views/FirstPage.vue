<template>
    <div class="page">
      <img src="@/assets/images/bg.jpg" alt="背景图" class="bg-image" />
    </div>
  </template>
  
  <script>
  export default {
    name: 'FirstPage',
  };
  </script>
  
  <style>
  .page {
    position: absolute;
    left: 200px;
    top: 0;
    right: 0;
    bottom: 0;
  }
  
  .bg-image {
    width: 100%;
    height: 100%;
    object-fit: cover;
  }
  </style>