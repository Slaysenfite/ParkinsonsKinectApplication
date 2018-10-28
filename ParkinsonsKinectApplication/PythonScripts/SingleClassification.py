#!/usr/bin/env python
# coding: utf-8

# In[83]:


import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import csv
from pathlib import Path
import os
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


# In[102]:


#Consts

joint_num = 60
data_dir_path = 'C://Users//user//Documents//Datasets//ParkinsonsDetectionDataset//'
project_dir_path = 'C://development//ParkinsonsKinectApplication//ParkinsonsKinectApplication//'
train_dir_path = 'C://development//ParkinsonsKinectApplication//ParkinsonsKinectApplication//TrainingData//'
temp_dir_path = 'C://development//ParkinsonsKinectApplication//ParkinsonsKinectApplication//Temp//'
pd_on = 'PD_ON//'
pd_off = 'PD_OFF//'
control = 'CONTROL//'

label_map = {'NONE':0,
           'REGULAR':1,
           'SEVERE':2}
label_names = ['NONE', 'REGULAR', 'SEVERE']

user_id = sys.args[1]

window_size = 10
w, h = 19, 3;


# In[103]:


def distance2D(x1, y1, x2, y2):
    return np.sqrt(np.square(x2 - x1) + np.square(y2 - y1))

def scaling(x1, y1, x2, y2):
    return distance2D(x1, y1, x2, y2)/100

def normalization(df_joints):
    for i, row in df_joints.iterrows():
        scaling_factor = scaling(row.SpineXPosition, row.SpineYPosition,row.ShoulderCenterXPosition, row.ShoulderCenterYPosition)/100        
        #scaling_factor = 1
        
        df_joints.set_value(i, 'HipCenterXPosition', (row.HipCenterXPosition - row.HipCenterXPosition)/scaling_factor)
        df_joints.set_value(i, 'HipCenterYPosition', (row.SpineYPosition - row.SpineYPosition)/scaling_factor)
        df_joints.set_value(i, 'HipCenterZPosition', (row.HipCenterZPosition - row.HipCenterZPosition))
        
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


# In[104]:


def savGolFilter(df_joints):
    for col in df_joints.columns:
        smoothed = savgol_filter(df_joints[col], 5, 2, mode='nearest') #Check correctness of paramters
        df_joints[col] = smoothed


# In[105]:


def windowSlicing(df_joints):
    windows = []
    r, c = df_joints.shape
    num_windows = int(r/window_size)
    for i in range(num_windows):
        a = i*window_size
        b = a + window_size
        df_sub = df_joints.iloc[a:b]
        windows.append(df_sub)
    return windows


# In[106]:


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
        
        
        df_points = pd.DataFrame(descriptor)
        descriptor_list.append(np.array(df_points))
       
    return descriptor_list


# In[107]:


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


# In[108]:


def flattenMatrix(matrix):
    rows, cols = np.array(matrix.shape)
    f_matrix = []
    i = 0
    for r in range(rows):
        for c in range(cols):
            f_matrix.append(matrix[r][c])
    return f_matrix
    
def writeResults(fname, cov):    
    output = ''
    f_cov = flattenMatrix(cov)
    with open(fname, 'a+') as f:
        wr = csv.writer(f, quoting=csv.QUOTE_ALL)
        wr.writerow(f_cov)

def process(dataFname, tempFname_p, tempFname_s, tempFname_a):
    #Read file, clean and normalise
    df_joints = cleanFile(dataFname)
    normalization(df_joints)
    savGolFilter(df_joints)
    
    pos_windows = windowSlicing(df_joints)
        
    pos_list = []
    vel_list = []
    acc_list = []
    
    cov_p_list = []
    cov_v_list = []
    cov_a_list = []

    for frame in pos_windows:
        pos_list.append(descriptor(frame, w, h, type = 'position'))
        vel_list.append(descriptor(frame.diff().dropna(), w, h, type = 'velocity'))
        acc_list.append(descriptor(frame.diff().diff().dropna(), w, h, type = 'acceleration'))
        
    iP,kP,jP,lP = np.array(pos_list).shape
    for i in range(iP):
        for k in range(kP):
            cov_p_list.append(np.cov(pos_list[i][k])) #gives a 3x3 cov matrix
            
    iP,kP,jP,lP = np.array(vel_list).shape
    for i in range(iP):
        for k in range(kP):        
            cov_v_list.append(np.cov(vel_list[i][k]))
            
    iP,kP,jP,lP = np.array(acc_list).shape
    for i in range(iP):
        for k in range(kP):        
            cov_a_list.append(np.cov(acc_list[i][k]))
      
    for p in cov_p_list:
        writeResults(tempFname_p, p)       
    for v in cov_v_list:
        writeResults(tempFname_s, v)
    for a in cov_a_list:
        writeResults(tempFname_a, a)


# In[109]:


def autoProcess(filelist, dir):
    for i in filelist:
        path = data_dir_path + dir + i
        with open(path, 'r') as f:
            if 'front' in f.name and user_id in f.name:
                process(f, temp_dir_path + user_id + "_temp_position_front.csv", 
                        temp_dir_path + user_id + "_temp_velocity_front.csv", 
                        temp_dir_path + user_id + "_temp_acc_front.csv")
            if 'back' in f.name and user_id in f.name:
                process(f, temp_dir_path + user_id + "_temp_position_back.csv", 
                        temp_dir_path + user_id + "_temp_velocity_back.csv", 
                        temp_dir_path + user_id + "_temp_acc_back.csv")
            if 'left' in f.name and user_id in f.name:
                process(f, temp_dir_path + user_id + "_temp_position_left.csv", 
                        temp_dir_path + user_id + "_temp_velocity_left.csv", 
                        temp_dir_path + user_id + "_temp_acc_left.csv")
            if 'right' in f.name and user_id in f.name:
                process(f, temp_dir_path + user_id + "_temp_position_right.csv", 
                        temp_dir_path + user_id + "_temp_velocity_right.csv", 
                        temp_dir_path + user_id + "_temp_acc_right.csv")
                
def autoCleanDir(filelist):
    for i in filelist:
        path = temp_dir_path + i
        os.unlink(path)


# In[110]:


import warnings
warnings.filterwarnings('ignore')
autoCleanDir(os.listdir(temp_dir_path))
autoProcess(os.listdir(data_dir_path + control), control)            


# In[118]:


def classify(train_filename, unknown_filename, sequence = "FRONT VIEW - POSITIONAL"):
    classification_data = pd.read_csv(train_filename, names=["0","1","2","3","4","5","6","7","8","label"])
    class_nums = np.array(classification_data['label'])
    del classification_data['label']
        
    X = np.array(classification_data)
    X_scaled = preprocessing.scale(X)
    
    y = label_binarize(class_nums, classes=[0,1,2])
    
    clf = svm.SVC(kernel='linear')
    clf.fit(X_scaled, class_nums)
    
    df_unknown = pd.read_csv(unknown_filename, names=["0","1","2","3","4","5","6","7","8"])
    unknown = np.array(df_unknown)
    
    pred_list = []
    
    rows, cols = unknown.shape
    for r in range(rows):
        pred = clf.predict([unknown[r]])
        if 'NONE' in pred:
            pred_list.append(0)
        if 'REGULAR' in pred:
            pred_list.append(1)
        if 'SEVERE' in pred:
            pred_list.append(2)
    
    modal_val = max(set(pred_list), key=pred_list.count)
    return modal_val


# In[119]:


def autoDataLoader(filelist):
    class_mode = []
    for i in filelist:
        path = temp_dir_path + i
        print(path)
        if 'front' in path and 'position' in path:
            class_mode.append(classify(project_dir_path + "TrainingData" + "//train_position_front.csv", path))
        if 'back' in path and 'position' in path:
            class_mode.append(classify(project_dir_path + "TrainingData" + "//train_position_back.csv", path, sequence = "BACK VIEW - POSITIONAL"))
        if 'left' in path and 'position' in path:
            class_mode.append(classify(project_dir_path + "TrainingData" + "//train_position_left.csv", path, sequence = "LEFT VIEW - POSITIONAL"))
        if 'right' in path and 'position' in path:
            class_mode.append(classify(project_dir_path + "TrainingData" + "//train_position_right.csv", path, sequence = "RIGHT VIEW - POSITIONAL"))
        if 'front' in path and 'velocity' in path:
            class_mode.append(classify(project_dir_path + "TrainingData" + "//train_velocity_front.csv", path, sequence = "FRONT VIEW - VELOCITY"))
        if 'back' in path and 'velocity' in path:
            class_mode.append(classify(project_dir_path + "TrainingData" + "//train_velocity_back.csv", path, sequence = "BACK VIEW - VELOCITY"))
        if 'left' in path and 'velocity' in path:
            class_mode.append(classify(project_dir_path + "TrainingData" + "//train_velocity_left.csv", path, sequence = "LEFT VIEW - VELOCITY"))
        if 'right' in path and 'velocity' in path:
            class_mode.append(classify(project_dir_path + "TrainingData" + "//train_velocity_right.csv", path, sequence = "RIGHT VIEW - VELOCITY"))
        if 'front' in path and 'acc' in path:
            class_mode.append(classify(project_dir_path + "TrainingData" + "//train_acc_front.csv", path, sequence = "FRONT VIEW - ACCELERATION"))
        if 'back' in path and 'acc' in path:
            class_mode.append(classify(project_dir_path + "TrainingData" + "//train_acc_back.csv", path, sequence = "BACK VIEW - ACCELERATION"))
        if 'left' in path and 'acc' in path:
            class_mode.append(classify(project_dir_path + "TrainingData" + "//train_acc_left.csv", path, sequence = "LEFT VIEW - ACCELERATION"))
        if 'right' in path and 'acc' in path:
            class_mode.append(classify(project_dir_path + "TrainingData" + "//train_acc_right.csv", path, sequence = "RIGHT VIEW - ACCELERATION"))
    return max(set(class_mode), key=class_mode.count)        
print(autoDataLoader(os.listdir(temp_dir_path)))


# In[ ]:




