"""
从pb文件中尝试导出数据
"""



import tensorflow as tf
from PIL import Image
from tensorflow.python.platform import gfile
import numpy as np
import matplotlib.pyplot as plt
import os
import utils

def plot_conv_weights(weights, name, channels_all=True):
    """
    Plots convolutional filters
    :param weights: numpy array of rank 4
    :param name: string, name of convolutional layer
    :param channels_all: boolean, optional
    :return: nothing, plots are saved on the disk
    """
    # make path to output folder
    plot_dir = os.path.join('E:/VisualStudio/VisualScript/utils/mnist/raw', 'conv_weights')
    plot_dir = os.path.join(plot_dir, name)

    # create directory if does not exist, otherwise empty it
    utils.prepare_dir(plot_dir, empty=True)

    w_min = np.min(weights)
    w_max = np.max(weights)

    channels = [0]
    # make a list of channels if all are plotted
    if channels_all:
        channels = range(weights.shape[2])

    # get number of convolutional filters
    num_filters = weights.shape[3]

    # get number of grid rows and columns
    grid_r, grid_c = utils.get_grid_dim(num_filters)

    # create figure and axes
    fig, axes = plt.subplots(min([grid_r, grid_c]),
                             max([grid_r, grid_c]))

    # iterate channels
    for channel in channels:
        # iterate filters inside every channel
        for l, ax in enumerate(axes.flat):
            # get a single filter
            img = weights[:, :, channel, l]
            # put it on the grid
            ax.imshow(img, vmin=w_min, vmax=w_max, interpolation='nearest', cmap='seismic')
            # remove any labels from the axes
            ax.set_xticks([])
            ax.set_yticks([])
        # save figure
        plt.savefig(os.path.join(plot_dir, '{}-{}.png'.format(name, channel)), bbox_inches='tight')

def plot_conv_output(conv_img, name):
    """
    Makes plots of results of performing convolution
    :param conv_img: numpy array of rank 4
    :param name: string, name of convolutional layer
    :return: nothing, plots are saved on the disk
    """
    # make path to output folder
    plot_dir = os.path.join('E:/VisualStudio/VisualScript/utils/mnist/raw', 'conv_output')
    plot_dir = os.path.join(plot_dir, name)

    # create directory if does not exist, otherwise empty it
    utils.prepare_dir(plot_dir, empty=True)

    w_min = np.min(conv_img)
    w_max = np.max(conv_img)

    # get number of convolutional filters
    num_filters = conv_img.shape[3]

    # get number of grid rows and columns
    grid_r, grid_c = utils.get_grid_dim(num_filters)

    # create figure and axes
    fig, axes = plt.subplots(min([grid_r, grid_c]),
                             max([grid_r, grid_c]))

    # iterate filters
    for l, ax in enumerate(axes.flat):
        # get a single image
        img = conv_img[0, :, :, l]
        # put it on the grid
        ax.imshow(img, vmin=w_min, vmax=w_max, interpolation='bicubic', cmap='Greys')
        # remove any labels from the axes
        ax.set_xticks([])
        ax.set_yticks([])
    # save figure
    plt.savefig(os.path.join(plot_dir, '{}.png'.format(name)), bbox_inches='tight')

class Predict(object):

    def __init__(self):
        # 清除默认图的堆栈，并设置全局图为默认图
        # 若不进行清楚则在第二次加载的时候报错，因为相当于重新加载了两次
        tf.reset_default_graph()
        gpu_options = tf.GPUOptions(per_process_gpu_memory_fraction=0.5)
        self.sess = tf.Session(config=tf.ConfigProto(gpu_options=gpu_options))
        self.sess.run(tf.global_variables_initializer())

        # 加载模型到sess中
        self.restore_mode_pb('E:/VisualStudio/VisualScript/utils/mnist/out/model/model_minimal.pb')
        print('load susess')


    def restore_mode_pb(self,pb_file_path):
        self.sess = tf.Session()
        with gfile.FastGFile(pb_file_path, 'rb') as f:
            graph_def = tf.GraphDef()
            graph_def.ParseFromString(f.read())
            self.sess.graph.as_default()
            tf.import_graph_def(graph_def, name='')

    def predict2(self, image_path):
        # 读取图片
        img = Image.open(image_path).convert('L')
        tv = list(img.getdata())
        tva = [(255 - x) * 1.0 / 255.0 for x in tv]
        x1 = [tva]

        out = self.sess.graph.get_tensor_by_name('out:0')
        input_x = self.sess.graph.get_tensor_by_name('input:0')
        Keep_prob = self.sess.graph.get_tensor_by_name('Keep_prob:0')

        w_conv1 = self.sess.graph.get_tensor_by_name('w1:0')
        tf.add_to_collection('conv1_weights', w_conv1)

        h_conv1 = self.sess.graph.get_tensor_by_name('activation1:0')
        tf.add_to_collection('conv1_output', h_conv1)

        prediction = tf.argmax(out, 1)
        predint = prediction.eval(feed_dict={input_x: x1, Keep_prob: 1.0}, session=self.sess)

        print(image_path)
        print(' Predict digit', predint[0])
        conv_weights = self.sess.run([tf.get_collection('conv1_weights')])
        for i, c in enumerate(conv_weights[0]):
            plot_conv_weights(c, 'conv{}'.format(i))

        conv_out = self.sess.run([tf.get_collection('conv1_output')], feed_dict={input_x: x1})
        for i, c in enumerate(conv_out[0]):
            plot_conv_output(c, 'conv{}'.format(i))

if __name__ == '__main__':
    model = Predict()
    model.predict2('E:/VisualStudio/VisualScript/utils/mnist/raw/2.png')
    # model.predict2('E:/VisualStudio/VisualScript/utils/mnist/raw/4.png')
    # model.predict2('E:/VisualStudio/VisualScript/utils/mnist/raw/8.png')

""
"将事先收集的信息画出来,画权重不需要给出输入信息,画输出需要给出输入信息"

# tf.add_to_collection('conv1_weights', w_conv1)
# tf.add_to_collection('conv1_output', h_conv1)
#
# conv_weights = sess.run([tf.get_collection('conv1_weights')])
# for i, c in enumerate(conv_weights[0]):
#     plot_conv_weights(c, 'conv{}'.format(i))
#
# conv_out = sess.run([tf.get_collection('conv1_output')], feed_dict={x: mnist.test.images[:1]})
# for i, c in enumerate(conv_out[0]):
#     plot_conv_output(c, 'conv{}'.format(i))