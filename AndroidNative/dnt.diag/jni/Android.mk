LOCAL_PATH := $(call my-dir)

include $(CLEAR_VARS)

LOCAL_MODULE    := dntdiag
LOCAL_CFLAGS := -DTOMIC_DEVICE_V1
LOCAL_CXXFLAGS := -DTOMIC_DEVICE_V1
LOCAL_SRC_FILES := dntdiag.cpp

include $(BUILD_SHARED_LIBRARY)
