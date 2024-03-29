import axios, { AxiosError, AxiosResponse } from "axios";
import { Activity, ActivityFormValues } from "../models/activity";
import { toast } from "react-toastify";
import { redirect, useNavigate } from "react-router-dom";
import { store } from "../strores/store";
import { User, UserFormValues } from "../models/user";
import Token from "../models/token";
import { Photo, Profile } from "../models/profile";
import { PaginatedResult } from "../models/Pagination";


const sleep = (delay: number) => {
    return new Promise((resolve, reject) => {
        setTimeout(resolve, delay);
    });
}


axios.defaults.baseURL = 'https://localhost:5001/api';

axios.interceptors.request.use(config => {
    const token = store.commonStore.token;
    if (token) {
        config.headers.Authorization = `Bearer ${token}`
    }
    return config;
})

axios.interceptors.response.use(async response => {

    await sleep(1000);
    const pagination = response.headers['pagination'];
    if (pagination) {
        response.data = new PaginatedResult(response.data, JSON.parse(pagination));
        return response as AxiosResponse<PaginatedResult<any>>
    }
    return response;
}, (error: AxiosError) => {
    const { data, status, config }: { data: any, status: number, config: any } = error.response!;
    console.log(error.response);
    switch (status) {
        case 400:
            if(typeof data === "string") {
                toast.error(data);
            }
            if (config.method === 'get' && data.errors.hasOwnProperty('id')) {
                window.location.replace("/not-found");
            }
            if (data.errors) {
                const modalStateErrors = [];
                for (const key in data.errors) {
                    if (data.errors[key]) {
                        modalStateErrors.push(data.errors[key])
                    }
                }
                throw modalStateErrors.flat();
            } 
            break;
        case 401:
            toast.error('Unauthorized');
            break;
        case 404:
            window.location.replace("/not-found");
            break;
        case 500:
            store.commonStore.setServrError(data);
            window.location.replace("/server-error");
            break;
        default:
            toast.error('Error')
            break;
    }
    return Promise.reject(error);
})

const responseBody = <T> (response: AxiosResponse<T>) => response.data;

const requests = {
    get: <T>(url: string) => axios.get<T>(url).then(responseBody),
    post: <T>(url: string, body: {}) => axios.post<T>(url, body).then(responseBody),
    put: <T>(url: string, body: {}) => axios.put<T>(url, body).then(responseBody),
    del: <T>(url: string) => axios.delete<T>(url).then(responseBody),
}

const Activities = {
    list: (params: URLSearchParams) => axios.get<PaginatedResult<Activity[]>>('/activities', {params}).then(responseBody),
    details: (id: string) => requests.get<Activity>(`/activities/${id}`),
    create: (activity: ActivityFormValues) => requests.post<void>(`/activities`, activity),
    update: (activity: ActivityFormValues) => requests.put<void>(`/activities/${activity.id}`, activity),
    delete: (id: string) => requests.del<void>(`/activities/${id}`),
    attend: (id: string) => requests.post<void>(`/activities/${id}/attend`, {}) 
}
const Account = {
    current: () => requests.get<User>('/authentication/getCurrentUser'),
    login: (user: UserFormValues) => requests.post<User>('/authentication/signin', user),
    register: (user: UserFormValues) => requests.post<User>('/authentication/signup', user)
}

const Profiles = {
    get: (username: string) => requests.get<Profile>(`/profiles/${username}`),
    uploadPhoto: (file: Blob) => {
        let formData = new FormData();
        formData.append('File', file);
        return axios.post<Photo>('photos', formData, {
            headers: { 'Content-Type': 'multipart/form-data' }
        });
    },
    setMainPhoto: (id: string) => requests.post(`/photos/${id}/setMain`, {}),
    deletePhoto: (id: string) => requests.del(`/photos/${id}`)
};

const agent = {
    Activities,
    Account,
    Profiles
};
export default agent;