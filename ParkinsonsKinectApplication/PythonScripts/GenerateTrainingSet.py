
# coding: utf-8

# In[253]:


import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
from pathlib import Path
import os
import sys 
from scipy.spatial import distance
from scipy.signal import savgol_filter
from statistics import mean, stdev
from scipy import linalg, interp
from sklearn import svm, datasets, preprocessing
from sklearn.metrics import roc_curve, auc, confusion_matrix, accuracy_score, precision_score, recall_score, classification_report
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import label_binarize
from sklearn.multiclass import OneVsRestClassifier
from sklearn.model_selection import StratifiedKFold
from sklearn.decomposition import PCA as sklearnPCA
from scipy.fftpack import fft, dct
from itertools import cycle
import itertools
import warnings
warnings.filterwarnings('ignore')

#Consts

joint_num = 60
project_dir_path = sys.argv[1]
data_dir_path = sys.argv[2]
train_dir_path = sys.argv[3]
pd_on = 'PD_ON//'
pd_off = 'PD_OFF//'
control = 'CONTROL//'

label_map = {'NONE':0,
           'REGULAR':1,
           'SEVERE':2}
label_names = ['NONE', 'REGULAR', 'SEVERE']

epsilon, kappa, heta = 0.3, 10, 10
w, h = 19, 3;


# In[254]:


def distance2D(x1, y1, x2, y2):
    return np.sqrt(np.square(x2 - x1) + np.square(y2 - y1))

def scaling(x1, y1, x2, y2):
    return distance2D(x1, y1, x2, y2)/100


# In[255]:


def cleanFile(fname):
    idx = []

    df_joint_data = pd.read_csv(fname, skipinitialspace=True).drop_duplicates().reset_index(drop=True)[:-5]

    if 'drop_this_col' in df_joint_data.columns:
        del df_joint_data['drop_this_col']

    for i in range(len(df_joint_data.index)):
        idx.append(i*0.033) #Could make this stochastic

    df_index = pd.DataFrame({'time': idx})
    df_joints = df_index.join(df_joint_data)

    df_joints.set_index('time', inplace=True)

    if (len(df_joints.columns)) > joint_num:
        df_joints.drop(df_joints.iloc[:, 60:1892], inplace=True, axis=1)

    if 'ShoulderLeftX Position' in df_joints.columns:    
        df_joints = df_joints.rename(columns={'ShoulderLeftX Position': 'ShoulderLeftXPosition'})

    return df_joints 


# In[256]:


def normalization(df_joints):
    for i, row in df_joints.iterrows():
        scaling_factor = scaling(row.SpineXPosition, row.SpineYPosition, row.ShoulderCenterXPosition, row.ShoulderCenterYPosition)/100
        
        df_joints.set_value(i, 'SpineXPosition', (row.SpineXPosition - row.HipCenterXPosition)/scaling_factor)
        df_joints.set_value(i, 'SpineYPosition', (row.SpineYPosition - row.HipCenterYPosition)/scaling_factor)
        df_joints.set_value(i, 'SpineZPosition', (row.SpineZPosition - row.HipCenterZPosition))
        
        df_joints.set_value(i, 'ShoulderCenterXPosition', (row.ShoulderCenterXPosition - row.HipCenterXPosition)/scaling_factor)
        df_joints.set_value(i, 'ShoulderCenterYPosition', (row.ShoulderCenterYPosition - row.HipCenterYPosition)/scaling_factor)
        df_joints.set_value(i, 'ShoulderCenterZPosition', (row.ShoulderCenterZPosition - row.HipCenterZPosition))
        
        df_joints.set_value(i, 'HeadXPosition', (row.HeadXPosition - row.HipCenterXPosition)/scaling_factor)
        df_joints.set_value(i, 'HeadYPosition', (row.HeadYPosition - row.HipCenterYPosition)/scaling_factor)
        df_joints.set_value(i, 'HeadZPosition', (row.HeadZPosition - row.HipCenterZPosition))
        
        df_joints.set_value(i, 'ShoulderLeftXPosition', (row.ShoulderLeftXPosition - row.HipCenterXPosition)/scaling_factor)
        df_joints.set_value(i, 'ShoulderLeftYPosition', (row.ShoulderLeftYPosition - row.HipCenterYPosition)/scaling_factor)
        df_joints.set_value(i, 'ShoulderLeftZPosition', (row.ShoulderLeftZPosition - row.HipCenterZPosition))
        
        df_joints.set_value(i, 'ElbowLeftXPosition', (row.ElbowLeftXPosition - row.HipCenterXPosition)/scaling_factor)
        df_joints.set_value(i, 'ElbowLeftYPosition', (row.ElbowLeftYPosition - row.HipCenterYPosition)/scaling_factor)
        df_joints.set_value(i, 'ElbowLeftZPosition', (row.ElbowLeftZPosition - row.HipCenterZPosition))
        
        df_joints.set_value(i, 'WristLeftXPosition', (row.WristLeftXPosition - row.HipCenterXPosition)/scaling_factor)
        df_joints.set_value(i, 'WristLeftYPosition', (row.WristLeftYPosition - row.HipCenterYPosition)/scaling_factor)
        df_joints.set_value(i, 'WristLeftZPosition', (row.WristLeftZPosition - row.HipCenterZPosition))
        
        df_joints.set_value(i, 'HandLeftXPosition', (row.HandLeftXPosition - row.HipCenterXPosition)/scaling_factor)
        df_joints.set_value(i, 'HandLeftYPosition', (row.HandLeftYPosition - row.HipCenterYPosition)/scaling_factor)
        df_joints.set_value(i, 'HandLeftZPosition', (row.HandLeftZPosition - row.HipCenterZPosition))
        
        df_joints.set_value(i, 'ShoulderRightXPosition', (row.ShoulderRightXPosition - row.HipCenterXPosition)/scaling_factor)
        df_joints.set_value(i, 'ShoulderRightYPosition', (row.ShoulderRightYPosition - row.HipCenterYPosition)/scaling_factor)
        df_joints.set_value(i, 'ShoulderRightZPosition', (row.ShoulderRightZPosition - row.HipCenterZPosition))
        
        df_joints.set_value(i, 'ElbowRightXPosition', (row.ElbowRightXPosition - row.HipCenterXPosition)/scaling_factor)
        df_joints.set_value(i, 'ElbowRightYPosition', (row.ElbowRightYPosition - row.HipCenterYPosition)/scaling_factor)
        df_joints.set_value(i, 'ElbowRightZPosition', (row.ElbowRightZPosition - row.HipCenterZPosition))
        
        df_joints.set_value(i, 'WristRightXPosition', (row.WristRightXPosition - row.HipCenterXPosition)/scaling_factor)
        df_joints.set_value(i, 'WristRightYPosition', (row.WristRightYPosition - row.HipCenterYPosition)/scaling_factor)
        df_joints.set_value(i, 'WristRightZPosition', (row.WristRightZPosition - row.HipCenterZPosition))
        
        df_joints.set_value(i, 'HandRightXPosition', (row.HandRightXPosition - row.HipCenterXPosition)/scaling_factor)
        df_joints.set_value(i, 'HandRightYPosition', (row.HandRightYPosition - row.HipCenterYPosition)/scaling_factor)
        df_joints.set_value(i, 'HandRightZPosition', (row.HandRightZPosition - row.HipCenterZPosition))
        
        df_joints.set_value(i, 'HipLeftXPosition', (row.HipLeftXPosition - row.HipCenterXPosition)/scaling_factor)
        df_joints.set_value(i, 'HipLeftYPosition', (row.HipLeftYPosition - row.HipCenterYPosition)/scaling_factor)
        df_joints.set_value(i, 'HipLeftZPosition', (row.HipLeftZPosition - row.HipCenterZPosition))
        
        df_joints.set_value(i, 'KneeLeftXPosition', (row.KneeLeftXPosition - row.HipCenterXPosition)/scaling_factor)
        df_joints.set_value(i, 'KneeLeftYPosition', (row.KneeLeftYPosition - row.HipCenterYPosition)/scaling_factor)
        df_joints.set_value(i, 'KneeLeftZPosition', (row.KneeLeftZPosition - row.HipCenterZPosition))
        
        df_joints.set_value(i, 'AnkleLeftXPosition', (row.AnkleLeftXPosition - row.HipCenterXPosition)/scaling_factor)
        df_joints.set_value(i, 'AnkleLeftYPosition', (row.AnkleLeftYPosition - row.HipCenterYPosition)/scaling_factor)
        df_joints.set_value(i, 'AnkleLeftZPosition', (row.AnkleLeftZPosition - row.HipCenterZPosition))
        
        df_joints.set_value(i, 'FootLeftXPosition', (row.FootLeftXPosition - row.HipCenterXPosition)/scaling_factor)
        df_joints.set_value(i, 'FootLeftYPosition', (row.FootLeftYPosition - row.HipCenterYPosition)/scaling_factor)
        df_joints.set_value(i, 'FootLeftZPosition', (row.FootLeftZPosition - row.HipCenterZPosition))
        
        df_joints.set_value(i, 'HandRightXPosition', (row.HandRightXPosition - row.HipCenterXPosition)/scaling_factor)
        df_joints.set_value(i, 'HandRightYPosition', (row.HandRightYPosition - row.HipCenterYPosition)/scaling_factor)
        df_joints.set_value(i, 'HandRightZPosition', (row.HandRightZPosition - row.HipCenterZPosition))
        
        df_joints.set_value(i, 'KneeRightXPosition', (row.KneeRightXPosition - row.HipCenterXPosition)/scaling_factor)
        df_joints.set_value(i, 'KneeRightYPosition', (row.KneeRightYPosition - row.HipCenterYPosition)/scaling_factor)
        df_joints.set_value(i, 'KneeRightZPosition', (row.KneeRightZPosition - row.HipCenterZPosition))
        
        df_joints.set_value(i, 'AnkleRightXPosition', (row.AnkleRightXPosition - row.HipCenterXPosition)/scaling_factor)
        df_joints.set_value(i, 'AnkleRightYPosition', (row.AnkleRightYPosition - row.HipCenterYPosition)/scaling_factor)
        df_joints.set_value(i, 'AnkleRightZPosition', (row.AnkleRightZPosition - row.HipCenterZPosition))
        
        df_joints.set_value(i, 'FootRightXPosition', (row.FootRightXPosition - row.HipCenterXPosition)/scaling_factor)
        df_joints.set_value(i, 'FootRightYPosition', (row.FootRightYPosition - row.HipCenterYPosition)/scaling_factor)
        df_joints.set_value(i, 'FootRightZPosition', (row.FootRightZPosition - row.HipCenterZPosition))


# In[257]:


def savGolFilter(df_joints):
    for col in df_joints.columns:
        smoothed = savgol_filter(df_joints[col], 5, 2, mode='nearest') #Check correctness of paramters
        df_joints[col] = smoothed
        
def dctFilter(df_joints):
    return dct((fft(df_joints).real), 1)


# In[282]:


def descriptor(df_joints, w, h, type):
    descriptor_list = []

    descriptor = [[0 for x in range(w)] for y in range(h)] 

    for i, row in df_joints.iterrows():

        descriptor[0][0] = row.SpineXPosition 
        descriptor[0][1] = row.ShoulderCenterXPosition 
        descriptor[0][2] = row.HeadXPosition 
        descriptor[0][3] = row.ShoulderLeftXPosition 
        descriptor[0][4] = row.ElbowLeftXPosition 
        descriptor[0][5] = row.WristLeftXPosition 
        descriptor[0][6] = row.HandLeftXPosition 
        descriptor[0][7] = row.ShoulderRightXPosition 
        descriptor[0][8] = row.ElbowRightXPosition 
        descriptor[0][9] = row.WristRightXPosition 
        descriptor[0][10] = row.HandRightXPosition
        descriptor[0][11] = row.HipLeftXPosition
        descriptor[0][12] = row.KneeLeftXPosition
        descriptor[0][13] = row.AnkleLeftXPosition
        descriptor[0][14] = row.FootLeftXPosition
        descriptor[0][15] = row.HandRightXPosition
        descriptor[0][16] = row.KneeRightXPosition
        descriptor[0][17] = row.AnkleRightXPosition
        descriptor[0][18] = row.FootRightXPosition

        descriptor[1][0] = row.SpineYPosition 
        descriptor[1][1] = row.ShoulderCenterYPosition 
        descriptor[1][2] = row.HeadYPosition 
        descriptor[1][3] = row.ShoulderLeftYPosition 
        descriptor[1][4] = row.ElbowLeftYPosition 
        descriptor[1][5] = row.WristLeftYPosition 
        descriptor[1][6] = row.HandLeftYPosition 
        descriptor[1][7] = row.ShoulderRightYPosition 
        descriptor[1][8] = row.ElbowRightYPosition 
        descriptor[1][9] = row.WristRightYPosition 
        descriptor[1][10] = row.HandRightYPosition
        descriptor[1][11] = row.HipLeftYPosition
        descriptor[1][12] = row.KneeLeftYPosition
        descriptor[1][13] = row.AnkleLeftYPosition
        descriptor[1][14] = row.FootLeftYPosition
        descriptor[1][15] = row.HandRightYPosition
        descriptor[1][16] = row.KneeRightYPosition
        descriptor[1][17] = row.AnkleRightYPosition
        descriptor[1][18] = row.FootRightYPosition

        descriptor[2][0] = row.SpineZPosition 
        descriptor[2][1] = row.ShoulderCenterZPosition 
        descriptor[2][2] = row.HeadZPosition 
        descriptor[2][3] = row.ShoulderLeftZPosition 
        descriptor[2][4] = row.ElbowLeftZPosition 
        descriptor[2][5] = row.WristLeftZPosition 
        descriptor[2][6] = row.HandLeftZPosition 
        descriptor[2][7] = row.ShoulderRightZPosition 
        descriptor[2][8] = row.ElbowRightZPosition 
        descriptor[2][9] = row.WristRightZPosition 
        descriptor[2][10] = row.HandRightZPosition
        descriptor[2][11] = row.HipLeftZPosition
        descriptor[2][12] = row.KneeLeftZPosition
        descriptor[2][13] = row.AnkleLeftZPosition
        descriptor[2][14] = row.FootLeftZPosition
        descriptor[2][15] = row.HandRightZPosition
        descriptor[2][16] = row.KneeRightZPosition
        descriptor[2][17] = row.AnkleRightZPosition
        descriptor[2][18] = row.FootRightZPosition

        if type is 'position':
            df_points = pd.DataFrame(descriptor)
            descriptor_list.append(np.array(df_points.cov()))
        if type is 'velocity':
            df_speeds = pd.DataFrame(descriptor)
            descriptor_list.append(np.array(df_speeds.cov()))
        if type is 'acceleration':
            df_accs = pd.DataFrame(descriptor)
            descriptor_list.append(np.array(df_accs.cov()))
    return descriptor_list


# In[277]:


def dissimilarity(covA, covB):    
    eigenvalues = linalg.eigvals(covA, covB)
    for i in range(eigenvalues.size):
        sigma += np.log(eigenvalues[i])**2
    return np.sqrt(sigma)

def reduce(covA):
    sklearn_pca = sklearnPCA(n_components=3)
    return sklearn_pca.fit_transform(covA)
    


# In[283]:


def writeResults(fname, cov, label):
    output = ""

    for x in range(19):
        for y in range(3):
            output += str(cov[x][y]) + ","
    output += label + '\n'
    with open(fname, 'a+') as f:
        f.write(output)
        f.close()
        
def process(dataFname, trainFname_p, trainFname_s, trainFname_a, label):
    #Read file, clean and normalise
    df_joints = cleanFile(dataFname)
    normalization(df_joints)
    savGolFilter(df_joints)
        
    pos_list = []
    vel_list = []
    acc_list = []

    pos_list = descriptor(df_joints, w, h, type = 'position')
    vel_list = descriptor(df_joints.diff().dropna(), w, h, type = 'velocity')
    acc_list= descriptor(df_joints.diff().diff().dropna(), w, h, type = 'acceleration')

    P = np.array(pos_list)
    S = np.array(vel_list)
    A = np.array(acc_list)
        
    #Average Covariance Matrices Calculation
    cov_p = reduce(np.mean(P, axis=0))
    cov_g = reduce(np.mean(S, axis=0))
    cov_a = reduce(np.mean(A, axis=0))

    writeResults(trainFname_p, cov_p, label)
    writeResults(trainFname_s, cov_g, label)
    writeResults(trainFname_a, cov_g, label)   


def autoProcess(filelist, dir):
    for i in filelist:
        path = data_dir_path + dir + i
        if dir is control: 
            label = 'NONE'
        if dir is pd_on: 
            label = 'REGULAR'
        if dir is pd_off: 
            label = 'SEVERE'
        with open(path, 'r') as f:
            if 'front' in f.name:
                process(f, project_dir_path + "TrainingData" + "//train_position_front.csv", 
                        project_dir_path + "TrainingData" + "//train_velocity_front.csv", 
                        project_dir_path + "TrainingData" + "//train_acc_front.csv",label)
            if 'back' in f.name:
                process(f, project_dir_path + "TrainingData" + "//train_position_back.csv", 
                        project_dir_path + "TrainingData" + "//train_velocity_back.csv",
                        project_dir_path + "TrainingData" + "//train_acc_back.csv", label)
            if 'left' in f.name or 'LHS' in f.name:
                process(f, project_dir_path + "TrainingData" + "//train_position_left.csv", 
                        project_dir_path + "TrainingData" + "//train_velocity_left.csv",
                        project_dir_path + "TrainingData" + "//train_acc_left.csv", label)
            if 'right' in f.name or 'RHS' in f.name:
                process(f, project_dir_path + "TrainingData" + "//train_position_right.csv", 
                        project_dir_path + "TrainingData" + "//train_velocity_right.csv",
                        project_dir_path + "TrainingData" + "//train_acc_right.csv", label)

def autoCleanDir(filelist):
    for i in filelist:
        path = train_dir_path + i
        os.unlink(path)

autoCleanDir(os.listdir(train_dir_path))

autoProcess(os.listdir(data_dir_path + control), control)
autoProcess(os.listdir(data_dir_path + pd_on), pd_on)
autoProcess(os.listdir(data_dir_path + pd_off), pd_off)

print('DONE')

