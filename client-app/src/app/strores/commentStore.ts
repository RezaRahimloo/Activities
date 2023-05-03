import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { ChatComment } from "../models/comment";
import { makeAutoObservable, runInAction } from "mobx";
import { store } from "./store";

export default class CommentStore {

    comments: ChatComment[] = [];
    hubConnection: HubConnection | null = null;

    constructor() {
        makeAutoObservable(this);
    }

    createHubConnection = (activityId: string) => {
        if (store.activityStore.selectedActivity) {
            this.hubConnection = new HubConnectionBuilder()
                .withUrl('https://localhost:5001/chat?activityId=' + activityId, {
                    accessTokenFactory: () => store.userStore.user?.token!
                })
                .withAutomaticReconnect()
                .configureLogging(LogLevel.Information)
                .build();
            console.log("create conntection token: ",store.userStore.user);
            this.hubConnection.start().catch(err => console.log("Error establishing connection", err));

            this.hubConnection.on("LoadComments", (comments: ChatComment[]) => {
                if (comments === null || comments === undefined) {
                    comments = [];
                }
                comments.forEach(c => {
                    c.createdAt = new Date(c.createdAt + 'Z');
                    console.log(c.createdAt)
                });
                runInAction(() => this.comments = comments);
            });

            this.hubConnection.on("RecieveComment", (comment: ChatComment) => {
                console.log(comment.createdAt);
                comment.createdAt = new Date(comment.createdAt);
                runInAction(() => this.comments.unshift(comment));
            })
        }
    }

    stopHubConnection = () => {
        this.hubConnection?.stop().catch(error => console.log('Error stopping conntection: ', error));
    }

    clearCommetns = () => {
        this.comments = [];
        this.stopHubConnection();
    }

    addComment = async (values: any) => {
        values.activityId = store.activityStore.selectedActivity?.id;
        try {
            //invokes public async Task SendComment(Create.Command command)
            await this.hubConnection?.invoke("SendComment", values);
        } catch (error) {
            console.log(error);
        }
    }
}