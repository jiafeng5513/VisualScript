could not create cudnn handle: CUDNN_STATUS_INTERNAL_ERROR could not destroy cudnn handle

T:\src\github\tensorflow\tensorflow\stream_executor\cuda\cuda_dnn.cc:403] could not create cudnn handle: CUDNN_STATUS_INTERNAL_ERROR
T:\src\github\tensorflow\tensorflow\stream_executor\cuda\cuda_dnn.cc:370] could not destroy cudnn handle: CUDNN_STATUS_BAD_PARAM
T:\src\github\tensorflow\tensorflow\core\kernels\conv_ops.cc:712] Check failed: stream->parent()->GetConvolveAlgorithms( conv_parameters.ShouldIncludeWinogradNonfusedAlgo<T>(), &algorithms)


参考文献:https://blog.csdn.net/qq_35608277/article/details/80111332 


解决方案:

    gpu_options = tf.GPUOptions(per_process_gpu_memory_fraction=0.1)
    sess= tf.Session(config=tf.ConfigProto(gpu_options=gpu_options))
